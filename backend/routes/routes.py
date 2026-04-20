from collections import defaultdict
from logging import Logger
from typing import Any

from db.hands_db import DatabaseHands
from fastapi import APIRouter, WebSocket, WebSocketDisconnect
from get_routes import Get
from post_routes import Post
from services import Services
from ws_routes import WS


class Routes:
    def __init__(self, db_hands: DatabaseHands, services: Services, logger: Logger) -> None:
        self.router = APIRouter()
        self.db_hands = db_hands
        self.services = services
        self.get_routes = Get(self)
        self.post_routes = Post(self)
        self.ws_routes = WS(self)

        self._setup_routes()
        self.logger = logger

        self.rooms: dict[str, set[WebSocket]] = defaultdict(set)
        self.room_state: dict[str, dict[str, list[Any]]] = defaultdict(lambda: {"teams": []})

    def _setup_routes(self) -> None:
        for name in dir(self.get_routes):
            if not name.startswith("_") and callable(method := getattr(self.get_routes, name)):
                self.router.get(f'/{name}')(method)

        for name in dir(self.post_routes):
            if not name.startswith("_") and callable(method := getattr(self.post_routes, name)):
                self.router.post(f'/{name}')(method)

        for name in dir(self.ws_routes):
            if not name.startswith("_") and callable(method := getattr(self.ws_routes, name)):
                self.router.websocket(f'/ws/{name}/' + '{code}')(method)

    def get_token_payload(self, token: str | None) -> dict[str, Any] | None:
        if not token or not (payload := self.services.try_get_jwt_payload(token)):
            return None
        return payload

    async def ensure_room_loaded(self, code: str) -> None:
        if self.room_state[code]["teams"]:
            return
        room_info = await self.db_hands.get_room_info(code)
        if room_info:
            self.room_state[code]["teams"] = self.services.parse_room_info(room_info)['teams']

    async def broadcast_room(self, code: str) -> None:
        payload = {"type": "update", "teams": self.room_state[code]["teams"]}

        for ws in list(self.rooms[code]):
            try:
                await ws.send_json(payload)
            except WebSocketDisconnect:
                self.rooms[code].discard(ws)
