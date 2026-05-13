from fastapi import WebSocket
from typing import Any

from domain.team import Team
from uuid import UUID


class Room:
    def __init__(self, code: str) -> None:
        """
        :param code: Код комнаты
        """
        self.code: str = code.upper().strip()
        self.teams: dict[str, Team] = {}
        self.black_list: set[str] = set()
        self.sockets: set[WebSocket] = set()
        self.played_tracks: set[tuple[int, int]] = set()

    @property
    def team_names(self) -> list[str]:
        """
        :return: Список названий подключенных команд
        """
        return [team.name for team in self.teams.values()]

    def is_team_kicked(self, uuid: str) -> bool:
        return uuid in self.black_list

    async def update_team_name(self, uuid: str | None, new_name: str) -> str | None:
        """
        :param uuid: UUID команды, которой хотим обновить имя
                    (если команда еще не сознада, UUID сгенерируется здесь)
        :param new_name: Новое имя для команды
        :return: Строковое представление UUID для сохранения в куку
        """
        print(new_name)
        print(self.team_names)
        if new_name in self.team_names:
            return None

        if uuid not in self.teams:
            team = Team(new_name)
        else:
            team = self.teams.pop(uuid)
            team.name = new_name
        uuid = team.uuid
        self.teams[uuid] = team
        await self.send_updated_info()
        return str(uuid)

    async def remove_team(self, name: str) -> None:
        """
        :param name: Имя команды, которую будут кикать (должно быть уникально, иначе удалим первую)
        """
        for team in self.teams.values():
            if team.name == name:
                kicked_team = self.teams.pop(team.uuid)
                self.black_list.add(kicked_team.uuid) # Кикнутая команда добавляется в список заблокированных
                await self.send_updated_info()
                break

    def get_team_info(self, uuid: str) -> dict[str, str] | None:
        """
        :param uuid: UUID команды
        :return: Если команда найдена, то имя и очки команды
        """
        if uuid not in self.teams:
            return None
        team = self.teams[uuid]
        return {'name': team.name, 'score': team.score}

    def update_score(self, uuid: str, score: int) -> None:
        if uuid in self.teams:
            team = self.teams[uuid]
            team.score += score

    def discard(self, socket: WebSocket) -> None:
        self.sockets.discard(socket)

    def close_room(self) -> None:
        for socket in self.sockets:
            socket.close()

    def add_played_track(self, row_index: int, column_index: int) -> None:
        self.played_tracks.add((row_index, column_index))

    def add_socket(self, socket: WebSocket) -> None:
        self.sockets.add(socket)

    async def send_updated_info(self) -> None:
        payload = {
            'type': 'update',
            'teams': self.team_names,
            'kicked': self.black_list,
        }
        await self.send_payload_to_all(payload)

    async def broadcast_start(self) -> None:
        payload = {
            'type': 'game_started',
            'teams': self.team_names,
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
