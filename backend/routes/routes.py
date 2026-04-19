from collections import defaultdict
from logging import Logger
from typing import Any

from api_schemes import LoginSchema, RegisterSchema, SaveGameSchema, TeamSchema
from db.hands_db import DatabaseHands
from fastapi import APIRouter, Cookie, HTTPException, Response, WebSocket, WebSocketDisconnect, status
from services import Services


class Routes:
    def __init__(self, db_hands: DatabaseHands, services: Services, logger: Logger) -> None:
        self.router = APIRouter()
        self.db_hands = db_hands
        self.services = services
        self.get_routes = Routes.Get(self)
        self.post_routes = Routes.Post(self)
        self.ws_routes = Routes.WS(self)

        self._setup_routes()
        self.logger = logger

        self.rooms: dict[str, set[WebSocket]] = defaultdict(set)
        self.room_state: dict[str, dict[str, list[Any]]] = defaultdict(lambda: {"teams": []})

    class Post:
        def __init__(self, outer: 'Routes'):
            self.outer = outer

        async def login(self, data: LoginSchema, response: Response):
            pass_hash = self.outer.services.get_hex_hash(data.password)
            user = await self.outer.db_hands.get_user(data.email)

            if not user or user[2] != pass_hash:
                raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

            _id, _name, _pass_hash, _email = user
            token = self.outer.services.get_jwt_token(_id, _name, _email)
            response.set_cookie("token", token, httponly=True, samesite="lax")
            return True

        async def register(self, data: RegisterSchema, response: Response):
            user = await self.outer.db_hands.get_user(data.email)
            if user:
                raise HTTPException(
                    status_code=status.HTTP_400_BAD_REQUEST, detail="A user with this email already exists"
                )

            pass_hash = self.outer.services.get_hex_hash(data.password)
            _id = await self.outer.db_hands.insert_user(data.name, pass_hash, data.email)

            token = self.outer.services.get_jwt_token(_id, data.name, data.email)
            response.set_cookie("token", token, httponly=True, samesite="lax")

            return {"name": data.name, "email": data.email}

        async def get_room_settings(self, data: SaveGameSchema):
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

        async def set_team_name(self, data: TeamSchema):
            is_new = await self.outer.db_hands.insert_or_update_team(data.id, data.uuid, data.name)
            code = data.code
            await self.outer._ensure_room_loaded(code)

            teams = self.outer.room_state[code]["teams"]

            if is_new:
                if data.name not in teams:
                    teams.append(data.name)
            else:
                for i, t in enumerate(teams):
                    if t == data.oldName:
                        teams[i] = data.name
                        break

            await self.outer._broadcast_room(code)
            return {"status": "ok", "new": is_new}

    class Get:
        def __init__(self, outer: 'Routes'):
            self.outer = outer

        async def get_all_user_games(self, token: str | None = Cookie(None)):
            payload = self.outer._get_token_payload(token)
            if not payload:
                raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")
            _id = int(payload['sub'])
            user_games = await self.outer.db_hands.get_all_user_games(_id)

            for game in user_games:
                game['scheduled_at'] = game['scheduled_at'].strftime("%Y-%m-%d %H:%M:%S")

            return user_games

        async def get_user_info(self, token: str | None = Cookie(None)):
            self.outer.logger.info("Что-то ищем")
            payload = self.outer._get_token_payload(token)
            if not payload:
                raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

            return {"email": payload["email"], "name": payload['name']}

        async def create_new_game(self):
            max_times = 0
            while max_times < 10:
                code = self.outer.services.generate_new_code()
                game_info = await self.outer.db_hands.get_game_info(code)
                if game_info is None:
                    return code
                max_times += 1

            raise HTTPException(
                status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
                detail="Could not generate unique code. Try again later.",
            )

        async def get_room_settings(self, code: str = ''):
            code = code.upper().strip()
            game = await self.outer.db_hands.get_game_info(code)
            if not game:
                return {"exists": False}
            return {"exists": True, "roomCode": code, **game}

        async def get_room_info(self, code: str):
            code = code.upper().strip()
            room_info = await self.outer.db_hands.get_room_info(code)
            return self.outer.services.parse_room_info(room_info) if room_info else None

        async def get_team_name(self, uuid: str) -> Response:
            return await self.outer.db_hands.get_team_name(uuid)

        async def run_game(self, code: str = ''):
            code = code.upper().strip()
            await self.outer.db_hands.update_game_any_param(code, "status", "waiting")
            room_info = await self.outer.db_hands.get_room_info(code)
            return self.outer.services.parse_room_info(room_info) if room_info else None

    class WS:
        def __init__(self, outer: 'Routes'):
            self.outer = outer

        async def room_ws(self, websocket: WebSocket, code: str):
            code = code.upper().strip()

            await websocket.accept()
            self.outer.rooms[code].add(websocket)

            await self.outer._ensure_room_loaded(code)

            await websocket.send_json({"type": "init", "teams": self.outer.room_state[code]["teams"]})

            try:
                while True:
                    await websocket.receive_json()
            except WebSocketDisconnect:
                self.outer.rooms[code].discard(websocket)

    def _setup_routes(self):
        for name in dir(self.get_routes):
            if not name.startswith("_") and callable(method := getattr(self.get_routes, name)):
                self.router.get(f'/{name}')(method)

        for name in dir(self.post_routes):
            if not name.startswith("_") and callable(method := getattr(self.post_routes, name)):
                self.router.post(f'/{name}')(method)

        for name in dir(self.ws_routes):
            if not name.startswith("_") and callable(method := getattr(self.ws_routes, name)):
                self.router.websocket(f'/ws/{name}/' + '{code}')(method)

    def _get_token_payload(self, token) -> dict[str, Any] | None:
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
