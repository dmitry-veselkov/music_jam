from fastapi import APIRouter, HTTPException, Response, status, Cookie, WebSocket, WebSocketDisconnect
from collections import defaultdict
from logging import Logger

from services import Services
from db.hands_db import DatabaseHands
from api_schemes import LoginSchema, RegisterSchema, SaveGameSchema, TeamSchema
from datetime import datetime

class ApiRouter:
    SONG_FIELDS = {"categories", "costs", "tracks"}

    def __init__(self, db_hands: DatabaseHands, services: Services, logger: Logger):
        self.router = APIRouter()
        self.db_hands = db_hands
        self.services = services
        self._setup_routes()
        self.logger = logger

        self.rooms: dict[str, set[WebSocket]] = defaultdict(set)
        self.room_state: dict[str, dict] = defaultdict(lambda: {"teams": []})

    def _setup_routes(self):
        @self.router.post("/login")
        async def login(data: LoginSchema, response: Response):
            pass_hash = self.services.get_hex_hash(data.password)
            user = await self.db_hands.get_user(data.email)

            if not user or user[2] != pass_hash:
                raise HTTPException(
                    status_code=status.HTTP_401_UNAUTHORIZED,
                    detail="Invalid credentials"
                )

            _id, _name, _pass_hash, _email = user
            token = self.services.get_jwt_token(_id, _name, _email)
            response.set_cookie("token", token, httponly=True, samesite="lax")

            return True

        @self.router.post("/register")
        async def register(data: RegisterSchema, response: Response):
            user = await self.db_hands.get_user(data.email)
            if user:
                raise HTTPException(
                    status_code=status.HTTP_400_BAD_REQUEST,
                    detail="A user with this email already exists"
                )

            pass_hash = self.services.get_hex_hash(data.password)
            _id = await self.db_hands.insert_user(data.name, pass_hash, data.email)

            token = self.services.get_jwt_token(_id, data.name, data.email)
            response.set_cookie("token", token, httponly=True, samesite="lax")

            return {"name": data.name, "email": data.email}

        @self.router.get("/get_all_user_games")
        async def get_all_user_games(token: str | None = Cookie(None)):
            payload = self._get_token_payload(token)
            if not payload:
                raise HTTPException(
                    status_code=status.HTTP_401_UNAUTHORIZED,
                    detail="Invalid credentials"
                )
            _id = int(payload['sub'])
            user_games = await self.db_hands.get_all_user_games(_id)
            for game in user_games:
                game['scheduled_at'] = game['scheduled_at'].strftime("%Y-%m-%d %H:%M:%S")

            return user_games

        @self.router.get('/get_user_info')
        async def get_user_info(token: str | None = Cookie(None)):
            self.logger.info(f"Что-то ищем")
            payload = self._get_token_payload(token)
            if not payload:
                raise HTTPException(
                    status_code=status.HTTP_401_UNAUTHORIZED,
                    detail="Invalid credentials"
                )

            return {"email": payload["email"], "name": payload['name']}


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
                        code,
                        datetime.now()
                    )
                    return code
            raise HTTPException(
                status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
                detail="Could not generate unique code. Try again later."
            )


        @self.router.get('/gameSettings')
        async def get_room_settings(code: str = ''):
            print(code)
            code = code.upper().strip()
            game = await self.db_hands.get_game_info(code)
            songs = await self.db_hands.get_room_tracks(code)
            print(game, songs)
            if not game:
                return {"exists": False}
            return {"exists": True, "roomCode": code, **game, **songs}

        @self.router.post('/gameSettings')
        async def save_room_settings(data: SaveGameSchema):
            code = data.roomCode.upper().strip()
            if not code:
                raise HTTPException(
                    status_code=status.HTTP_400_BAD_REQUEST,
                    detail="roomCode is required"
                )
            dumped = data.model_dump()
            game_fields = {k: v for k, v in dumped.items() if k not in self.SONG_FIELDS}
            songs_data = {k: v for k, v in dumped.items() if k in self.SONG_FIELDS}

            for column, value in game_fields.items():
                await self.db_hands.update_game_any_param(code, column, value)

            if any(songs_data.values()):
                await self.db_hands.update_game_settings(code, songs_data)

            return {
                "success": True,
                "roomCode": code,
                "game": data
            }

        @self.router.get('/get_room_info')
        async def get_room_info(code: str):
            code = code.upper().strip()
            room_info = await self.db_hands.get_room_info(code)
            songs = await self.db_hands.get_room_tracks(code)
            return self.services.parse_room_info(room_info, songs) if room_info else None

        @self.router.post('/set_team_name')
        async def set_team_name(data: TeamSchema):
            is_new = await self.db_hands.insert_or_update_team(data.id, data.uuid, data.name)
            code = data.code
            await self._ensure_room_loaded(code)

            teams = self.room_state[code]["teams"]

            if is_new:
                if data.name not in teams:
                    teams.append(data.name)
            else:
                for i, t in enumerate(teams):
                    if t == data.oldName:
                        teams[i] = data.name
                        break

            await self._broadcast_room(code)
            return {"status": "ok", "new": is_new}

        @self.router.get('/get_team_name')
        async def get_team_name(uuid: str) -> Response:
            return await self.db_hands.get_team_name(uuid)

        @self.router.get('/run_game')
        async def run_game(code: str = ''):
            code = code.upper().strip()
            await self.db_hands.update_game_any_param(code, "status", "waiting")
            room_info = await self.db_hands.get_room_info(code)
            print(f"code={code!r}, room_info={room_info}")
            tracks_info = await self.db_hands.get_room_tracks(code)
            return self.services.parse_room_info(room_info, tracks_info) if room_info else None

        @self.router.websocket("/ws/room/{code}")
        async def room_ws(websocket: WebSocket, code: str):
            code = code.upper().strip()

            await websocket.accept()
            self.rooms[code].add(websocket)

            await self._ensure_room_loaded(code)

            await websocket.send_json({
                "type": "init",
                "teams": self.room_state[code]["teams"]
            })

            try:
                while True:
                    await websocket.receive_json()
            except WebSocketDisconnect:
                self.rooms[code].discard(websocket)

    def _get_token_payload(self, token):
        if not token:
            return None

        payload = self.services.try_get_jwt_payload(token)
        if not payload:
            return None

        return payload

    async def _ensure_room_loaded(self, code: str):
        if self.room_state[code]["teams"]:
            return
        room_info = await self.db_hands.get_room_info(code)
        tracks_info = await self.db_hands.get_room_tracks(code)
        if room_info:
            self.room_state[code]["teams"] = self.services.parse_room_info(room_info, tracks_info)['teams']


    async def _broadcast_room(self, code: str):
        payload = {
            "type": "update",
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
