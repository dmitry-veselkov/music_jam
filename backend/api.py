from collections import defaultdict
from fastapi.responses import StreamingResponse
from logging import Logger
from typing import Any

from db.hands_db import DatabaseHands
from fastapi import APIRouter, Cookie, HTTPException, Response, WebSocket, WebSocketDisconnect, status, UploadFile, \
    File, Form
from services import Services
from api_schemes import LoginSchema, RegisterSchema, SaveGameSchema, TeamSchema
from s3 import S3


class ApiRouter:
    SONG_FIELDS = {"categories", "costs", "tracks"}

    def __init__(self, db_hands: DatabaseHands, services: Services, logger: Logger, s3: S3):
        self.router = APIRouter()
        self.db_hands = db_hands
        self.services = services
        self._setup_routes()
        self.logger = logger
        self.s3 = s3

        self.rooms: dict[str, set[WebSocket]] = defaultdict(set)
        self.room_state: dict[str, dict[str, list[Any]]] = defaultdict(lambda: {"teams": []})

    def _setup_routes(self) -> None:
        @self.router.post("/login")
        async def login(data: LoginSchema, response: Response) -> bool:
            pass_hash = self.services.get_hex_hash(data.password)
            user = await self.db_hands.get_user(data.email)

            if not user or user[2] != pass_hash:
                raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

            _id, _name, _pass_hash, _email = user
            token = self.services.get_jwt_token(_id, _name, _email)
            response.set_cookie("token", token, httponly=True, samesite="lax")

            return True

        @self.router.post("/register")
        async def register(data: RegisterSchema, response: Response) -> dict[str, str]:
            user = await self.db_hands.get_user(data.email)
            if user:
                raise HTTPException(
                    status_code=status.HTTP_400_BAD_REQUEST, detail="A user with this email already exists"
                )

            pass_hash = self.services.get_hex_hash(data.password)
            _id = await self.db_hands.insert_user(data.name, pass_hash, data.email)

            token = self.services.get_jwt_token(_id, data.name, data.email)
            response.set_cookie("token", token, httponly=True, samesite="lax")

            return {"name": data.name, "email": data.email}

        @self.router.get("/get_all_user_games")
        async def get_all_user_games(token: str | None = Cookie(None)) -> list[dict[str, Any]]:
            payload = self._get_token_payload(token)
            if not payload:
                raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")
            _id = int(payload['sub'])
            user_games = await self.db_hands.get_all_user_games(_id)

            return user_games

        @self.router.get('/get_user_info')
        async def get_user_info(token: str | None = Cookie(None)) -> dict[str, Any]:
            payload = self._get_token_payload(token)
            if not payload:
                raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

            return {"email": payload["email"], "name": payload['name'], 'id': payload['sub']}

        @self.router.post('/create_new_game')
        async def create_new_game(token: str = Cookie(None)):
            payload = self.services.try_get_jwt_payload(token)
            user_id = int(payload["sub"])
            for max_times in range(10):
                code = self.services.generate_new_code()
                game_info = await self.db_hands.get_game_info(code)
                if game_info is None:
                    await self.db_hands.create_game(
                        user_id,
                        "Новая игра",
                        '',
                        code,
                    )
                    return code
            raise HTTPException(
                status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
                detail="Could not generate unique code. Try again later.",
            )

        @self.router.get('/gameSettings')
        async def get_room_settings(code: str = ''):
            code = code.upper().strip()
            game = await self.db_hands.get_game_info(code)
            if not game:
                return {"exists": False}
            songs = await self.db_hands.get_room_tracks(code)
            return {"exists": True, "roomCode": code, **game, **songs}

        @self.router.post('/gameSettings')
        async def save_room_settings(data: SaveGameSchema):
            code = data.roomCode.upper().strip()
            if not code:
                raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="roomCode is required")

            await self.db_hands.update_game_any_param(code, "title", data.title)
            await self.db_hands.update_game_any_param(code, "description", data.description)
            await self.db_hands.update_game_any_param(code, "mode", data.mode)

            print(data.tracks)
            await self.db_hands.save_game_grid(
                code,
                data.categories,
                data.costs,
                data.tracks
            )

            return {
                "success": True,
                "roomCode": code
            }

        @self.router.get('/get_room_info')
        async def get_room_info(code: str) -> dict[str, Any] | None:
            code = code.upper().strip()
            room_info = await self.db_hands.get_room_info(code)
            songs = await self.db_hands.get_room_tracks(code)
            return self.services.parse_room_info(room_info, songs) if room_info else None

        @self.router.post('/set_team_name')
        async def set_team_name(data: TeamSchema) -> dict[str, Any]:
            code = data.code
            # await self._ensure_room_loaded(code)
            teams = self.room_state[code]["teams"]

            if data.name not in teams:
                teams.append(data.name)
            else:
                for i, t in enumerate(teams):
                    if t == data.oldName:
                        teams[i] = data.name
                        break
            await self._broadcast_room(code)
            return {"status": "ok"}

        # @self.router.post('/add_points')
        # async def add_points(code, payload : AddPointsSchema):
        #     points = payload.points if payload.correct else -payload.points
        #     return await self.db_hands.update_score_team(code,
        #                                                  payload.team,
        #                                                  points)

        # @self.router.get('/get_team_name')
        # async def get_team_name(uuid: str) -> Response:
        #     return await self.db_hands.get_team_name(uuid)

        @self.router.get('/run_game')
        async def run_game(code: str = '') -> None | dict[str, Any]:
            code = code.upper().strip()
            await self.db_hands.update_game_any_param(code, "status", "waiting")
            room_info = await self.db_hands.get_room_info(code)
            tracks_info = await self.db_hands.get_room_tracks(code)
            return self.services.parse_room_info(room_info, tracks_info) if room_info else None

        @self.router.post('/start_game')
        async def start_game(code: str = ''):
            code = code.upper().strip()
            await self.db_hands.update_game_any_param(code, "status", "playing")
            await self._broadcast_start(code)
            return {
                "success": True
            }

        @self.router.post('/end_game')
        async def end_game(code: str = ''):
            code = code.upper().strip()
            await self.db_hands.update_game_any_param(code, "status", "ended")
            await self._broadcast_room_message(code, {"type": "game_ended"})
            return {
                "success": True
            }

        @self.router.post('/upload_music')
        async def upload_track(title: str = Form(...),
                               artist: str = Form(...),
                               category: str = Form(...),
                               cost: str = Form(...),
                               question: UploadFile = File(None),
                               answer: UploadFile = File(None),
                               question_url: str = Form(None),
                               answer_url: str = Form(None)):

            if not title.strip() or not artist.strip():
                return {"error": "Не все поля заполнены"}

            print(question_url, answer_url)
            if question:
                if not question.content_type.startswith("audio/"):
                    return {"error": "Файл вопроса не аудио!"}
                question_name = self.services.get_unique_s3_uuid(question.filename)
                self.s3.upload(question.file, question_name, question.content_type)
            elif question_url:
                question_name = question_url
            else:
                return {"error": "Нет файла или ссылки для вопроса"}

            if answer:
                if not answer.content_type.startswith("audio/"):
                    return {"error": "Файл вопроса не аудио!"}
                answer_name = self.services.get_unique_s3_uuid(answer.filename)
                self.s3.upload(answer.file, answer_name, answer.content_type)
            elif answer_url:
                answer_name = answer_url
            else:
                return {"error": "Нет файла или ссылки для ответа"}

            return {
                "status": "ok",
                "song": {
                    "title": title,
                    "artist": artist,
                    "questionUrl": question_name,
                    "answerUrl": answer_name
                }
            }

        @self.router.get("/play")
        async def play_song(file_name: str):
            body, content_type = self.s3.get_stream(file_name)
            return StreamingResponse(
                body.iter_chunks(chunk_size=1024 * 512),
                media_type=content_type)

        @self.router.websocket("/ws/room/{code}")
        async def room_ws(websocket: WebSocket, code: str) -> None:
            code = code.upper().strip()

            await websocket.accept()
            self.rooms[code].add(websocket)

            # await self._ensure_room_loaded(code)

            await websocket.send_json({"type": "init", "teams": self.room_state[code]["teams"]})

            try:
                while True:
                    msg = await websocket.receive_json()
                    code = code.upper().strip()

                    if msg['type'] in ('track_started', 'player_buzzed', 'team_answer', 'game_ended', 'show_answer',
                                       'add_points', 'reset_answer_btn'):
                        await self._broadcast_room_message(code, msg)

            except WebSocketDisconnect:
                self.rooms[code].discard(websocket)

    def _get_token_payload(self, token: str | None) -> dict[str, Any] | None:
        if not token or not (payload := self.services.try_get_jwt_payload(token)):
            return None
        return payload

    # async def _ensure_room_loaded(self, code: str):
    #     if self.room_state[code]["teams"]:
    #         return
    #     room_info = await self.db_hands.get_room_info(code)
    #     tracks_info = await self.db_hands.get_room_tracks(code)
    #     if room_info:
    #         self.room_state[code]["teams"] = self.services.parse_room_info(room_info, tracks_info)['teams']

    async def _broadcast_room(self, code: str) -> None:
        payload = {"type": "update", "teams": self.room_state[code]["teams"]}

        dead = []
        for ws in self.rooms[code]:
            try:
                await ws.send_json(payload)
            except:
                dead.append(ws)

        for ws in dead:
            self.rooms[code].discard(ws)

    async def _broadcast_room_message(self, code: str, payload: dict):
        dead = []
        for ws in self.rooms[code]:
            try:
                await ws.send_json(payload)
            except:
                dead.append(ws)

        for ws in dead:
            self.rooms[code].discard(ws)

    async def _broadcast_start(self, code: str):
        code = code.upper().strip()
        payload = {
            "type": "game_started",
            "teams": self.room_state[code]["teams"]
        }
        dead = []
        for ws in self.rooms[code]:
            try:
                await ws.send_json(payload)
            except:
                dead.append(ws)

        for ws in dead:
            self.rooms[code].discard(ws)
