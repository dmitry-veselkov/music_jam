from collections import defaultdict
from logging import Logger
from typing import Any

from api_schemes import LoginSchema, RegisterSchema, SaveGameSchema, TeamSchema
from db.hands_db import DatabaseHands
from fastapi import APIRouter, Cookie, HTTPException, Response, WebSocket, WebSocketDisconnect, status
from services import Services


class ApiRouter:
    def __init__(self, db_hands: DatabaseHands, services: Services, logger: Logger) -> None:
        self.router = APIRouter()
        self.db_hands = db_hands
        self.services = services
        self._setup_routes()
        self.logger = logger

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

            for game in user_games:
                game['scheduled_at'] = game['scheduled_at'].strftime("%Y-%m-%d %H:%M:%S")

            return user_games

        @self.router.get('/get_user_info')
        async def get_user_info(token: str | None = Cookie(None)) -> dict[str, Any]:
            self.logger.info("Что-то ищем")
            payload = self._get_token_payload(token)
            if not payload:
                raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

            return {"email": payload["email"], "name": payload['name']}

        @self.router.get('/get_new_game_code')
        async def create_new_game() -> str:
            max_times = 0
            while max_times < 10:
                code = self.services.generate_new_code()
                game_info = await self.db_hands.get_game_info(code)
                if game_info is None:
                    return code
                max_times += 1

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
            return {"exists": True, "roomCode": code, **game}

        @self.router.post('/gameSettings')
        async def get_room_settings(data: SaveGameSchema) -> dict[str, Any]:
            code = data.roomCode.upper().strip()
            if not code:
                raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="roomCode is required")

            print(data)

            # game_characteristics[code] = {
            #     'name': data.get('name', ''),
            #     'author': data.get('author', ''),
            #     'description': data.get('description', ''),
            #     'maxTeams': data.get('maxTeams', 4),
            #     'categories': data.get('categories', []),
            #     'costs': data.get('costs', []),
            #     'tracks': data.get('tracks', {})
            # }

            return {"success": True, "roomCode": code, "game": data}

        @self.router.get('/get_room_info')
        async def get_room_info(code: str) -> dict[str, Any] | None:
            code = code.upper().strip()
            room_info = await self.db_hands.get_room_info(code)
            return self.services.parse_room_info(room_info) if room_info else None

        @self.router.post('/set_team_name')
        async def set_team_name(data: TeamSchema) -> dict[str, Any]:
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
        async def get_team_name(uuid: str) -> Any | None:
            return await self.db_hands.get_team_name(uuid)

        @self.router.get('/run_game')
        async def run_game(code: str = '') -> None | dict[str, Any]:
            code = code.upper().strip()
            await self.db_hands.update_game_any_param(code, "status", "waiting")
            room_info = await self.db_hands.get_room_info(code)
            return self.services.parse_room_info(room_info) if room_info else None

        @self.router.websocket("/ws/room/{code}")
        async def room_ws(websocket: WebSocket, code: str) -> None:
            code = code.upper().strip()

            await websocket.accept()
            self.rooms[code].add(websocket)

            await self._ensure_room_loaded(code)

            await websocket.send_json({"type": "init", "teams": self.room_state[code]["teams"]})

            try:
                while True:
                    await websocket.receive_json()
            except WebSocketDisconnect:
                self.rooms[code].discard(websocket)

    def _get_token_payload(self, token: str | None) -> dict[str, Any] | None:
        if not token or not (payload := self.services.try_get_jwt_payload(token)):
            return None
        return payload

    async def _ensure_room_loaded(self, code: str) -> None:
        if self.room_state[code]["teams"]:
            return
        room_info = await self.db_hands.get_room_info(code)
        if room_info:
            self.room_state[code]["teams"] = self.services.parse_room_info(room_info)['teams']

    async def _broadcast_room(self, code: str) -> None:
        payload = {"type": "update", "teams": self.room_state[code]["teams"]}

        for ws in list(self.rooms[code]):
            try:
                await ws.send_json(payload)
            except WebSocketDisconnect:
                self.rooms[code].discard(ws)
