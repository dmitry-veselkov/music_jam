from collections import defaultdict
from fastapi.responses import StreamingResponse
from logging import Logger
from typing import Any

from db.hands_db import DatabaseHands
from fastapi import APIRouter, Cookie, HTTPException, Response, WebSocket, WebSocketDisconnect, status, UploadFile, \
    File, Form
from services import Services
from api_schemes import LoginSchema, RegisterSchema, SaveGameSchema, TeamSchema, RemoveTeamSchema, AddPointsSchema, \
    AddPlayedTrack
from s3 import S3
from domain.room import Room


class ApiRouter:
    SONG_FIELDS = {"categories", "costs", "tracks"}

    def __init__(self, db_hands: DatabaseHands, services: Services, logger: Logger, s3: S3):
        self.router = APIRouter()
        self.db_hands = db_hands
        self.services = services
        self._setup_routes()
        self.logger = logger
        self.s3 = s3
        self.rooms: dict[str, Room] = {}

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

        @self.router.post("/logout", status_code=204)
        async def logout_user(response: Response) -> None:
            response.delete_cookie("token")

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

        @self.router.get('/room_state')
        async def get_room_state(code: str):
            code = code.upper().strip()
            room = self.rooms.get(code)
            if not room:
                return {"teams": {}, "played_tracks": []}
            return {
                "teams": room.teams,
                "played_tracks": [list(t) for t in room.played_tracks]
            }

        @self.router.post('/give_points')
        async def give_points(data: AddPointsSchema):
            code = data.code.upper().strip()
            room = self.rooms.get(code)
            if room:
                room.update_score(data.team, data.points)
            return {"success": True}

        @self.router.post('/set_team_name')
        async def set_team_name(data: TeamSchema) -> dict[str, Any]:
            code = data.code
            # teams = self.rooms[code].teams.keys()
            # self.rooms[code].teams[data.oldName] = data.name
            if data.oldName not in self.rooms[code].teams:
                scores = 0
            else:
                scores = self.rooms[code].teams.pop(data.oldName)
            self.rooms[code].add_team(data.name, scores)

            await self.rooms[code].broadcast_room()
            return {"status": "ok"}

        @self.router.post('/remove_team')
        async def remove_team(data: RemoveTeamSchema) -> None:
            team_name, code = data.team_name, data.code
            self.rooms[code].remove_team(team_name)
            await self.rooms[code].broadcast_room()

        @self.router.post('/delete_game')
        async def delete_game(code: str = '') -> None:
            code = code.upper().strip()
            await self.db_hands.delete_game(code)

        @self.router.get('/run_game')
        async def run_game(code: str = '') -> None | dict[str, Any]:
            code = code.upper().strip()
            self.rooms[code] = Room(code)
            await self.db_hands.update_game_any_param(code, "status", "waiting")
            room_info = await self.db_hands.get_room_info(code)
            tracks_info = await self.db_hands.get_room_tracks(code)
            return self.services.parse_room_info(room_info, tracks_info) if room_info else None

        @self.router.post('/start_game')
        async def start_game(code: str = '') -> dict[str, bool]:
            code = code.upper().strip()
            await self.db_hands.update_game_any_param(code, "status", "playing")
            await self.rooms[code].broadcast_start()
            return {"success": True}

        @self.router.post('/end_game')
        async def end_game(code: str = '') -> dict[str, bool]:
            code = code.upper().strip()
            await self.db_hands.update_game_any_param(code, "status", "ended")
            await self.rooms[code].send_payload_to_all({"type": "game_ended"})
            self.rooms[code].close_room()
            self.rooms.pop(code)
            return {"success": True}

        @self.router.post('/add_played_track')
        async def add_played_track(data: AddPlayedTrack):
            code = data.code.upper().strip()
            self.rooms[code].add_played_track(data.row, data.column)
            return {"success": True}

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

            self.rooms[code].add_socket(websocket)
            teams_names = list(self.rooms[code].teams.keys())
            await websocket.send_json({"type": "init", "teams": teams_names})

            try:
                while True:
                    msg = await websocket.receive_json()
                    if msg['type'] in ('track_started', 'player_buzzed', 'team_answer', 'game_ended',
                                       'show_answer', 'add_points', 'reset_answer_btn'):
                        await self.rooms[code].send_payload_to_all(msg)
            except WebSocketDisconnect:
                room = self.rooms.get(code)
                if room:
                    self.rooms[code].discard(websocket)

    def _get_token_payload(self, token: str | None) -> dict[str, Any] | None:
        if not token or not (payload := self.services.try_get_jwt_payload(token)):
            return None
        return payload
