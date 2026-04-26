from routes import Routes
from fastapi import WebSocket, WebSocketDisconnect


class WS:
    def __init__(self, outer: Routes) -> None:
        self.outer = outer

    async def room_ws(self, websocket: WebSocket, code: str) -> None:
        code = code.upper().strip()

        await websocket.accept()
        self.outer.rooms[code].add(websocket)

        await self.outer.ensure_room_loaded(code)

        await websocket.send_json({"type": "init", "teams": self.outer.room_state[code]["teams"]})

        try:
            while True:
                await websocket.receive_json()
        except WebSocketDisconnect:
            self.outer.rooms[code].discard(websocket)
