from fastapi import WebSocket
from typing import Any


class Room:
    def __init__(self, code: str) -> None:
        self.code: str = code.upper().strip()
        self.teams: dict[str, int] = {}
        self.sockets: set[WebSocket] = set()
        self.played_tracks : set[tuple[int, int]] = set()

    def add_team(self, team_name: str, scores: int = 0) -> None:
        if team_name not in self.teams:
            self.teams[team_name] = scores

    def remove_team(self, team_name: str) -> None:
        self.teams.pop(team_name)

    def update_score(self, team_name: str, score: int) -> None:
        if team_name in self.teams:
            self.teams[team_name] += score

    def discard(self, socket: WebSocket) -> None:
        self.sockets.discard(socket)

    def close_room(self) -> None:
        for socket in self.sockets:
            socket.close()

    def add_played_track(self, row_index : int, column_index: int) -> None:
        self.played_tracks.add((row_index, column_index))

    def add_socket(self, socket: WebSocket) -> None:
        self.sockets.add(socket)

    async def broadcast_room(self) -> None:
        payload = {"type": "update", "teams": list(self.teams.keys())}
        await self.send_payload_to_all(payload)

    async def broadcast_start(self) -> None:
        payload = {
            "type": "game_started",
            "teams": list(self.teams.keys())
        }
        await self.send_payload_to_all(payload)

    async def send_payload_to_all(self, payload: dict[str, Any]) -> None:
        dead = []
        for ws in self.sockets:
            try:
                await ws.send_json(payload)
            except:
                dead.append(ws)
        for ws in dead:
            self.sockets.discard(ws)
