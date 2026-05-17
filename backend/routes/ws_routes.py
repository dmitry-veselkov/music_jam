from typing import TYPE_CHECKING

from fastapi import WebSocket, WebSocketDisconnect

if TYPE_CHECKING:
    from routes.routes import Routes


class WS:
    def __init__(self, outer: "Routes") -> None:
        self.outer = outer

    async def room_ws(self, websocket: WebSocket, code: str) -> None:
        code = code.upper().strip()
        await websocket.accept()

        room = self.outer.rooms.get(code)
        if not room:
            await websocket.close()
            return

        room.add_socket(websocket)
        await websocket.send_json({"type": "init", "teams": room.team_names})

        try:
            while True:
                msg = await websocket.receive_json()
                if msg['type'] in ('track_started', 'player_buzzed', 'team_answer', 'game_ended',
                                   'show_answer', 'add_points', 'reset_answer_btn'):
                    await room.send_payload_to_all(msg)
        except WebSocketDisconnect:
            room.discard(websocket)
