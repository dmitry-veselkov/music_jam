from fastapi import WebSocket
from typing import Any

from domain.team import Team


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

    @property
    def teams_scores(self) -> dict[str, int]:
        """
        :return: Словарь {название_команды: счет} — формат, который ожидает фронт
        """
        return {team.name: team.score for team in self.teams.values()}

    @property
    def kicked_uuids(self) -> list[str]:
        return list(self.black_list)

    def is_team_kicked(self, uuid: str | None) -> bool:
        if not uuid:
            return False
        return uuid in self.black_list

    def _is_name_taken(self, name: str, exclude_uuid: str | None = None) -> bool:
        for uuid, team in self.teams.items():
            if uuid == exclude_uuid:
                continue
            if team.name == name:
                return True
        return False

    async def update_team_name(self, uuid: str | None, new_name: str) -> str | None:
        """
        Создает новую команду или переименовывает существующую (по uuid из куки).

        :param uuid: UUID команды, которой хотим обновить имя
                    (если команда еще не создана, UUID сгенерируется здесь)
        :param new_name: Новое имя для команды
        :return: Строковое представление UUID для сохранения в куку.
                 None — если имя уже занято другой командой.
        """
        new_name = new_name.strip()
        if not new_name:
            return None

        if self._is_name_taken(new_name, exclude_uuid=uuid):
            return None

        if uuid and uuid in self.teams:
            team = self.teams[uuid]
            team.name = new_name
            result_uuid = uuid
        else:
            team = Team(new_name)
            result_uuid = str(team.uuid)
            self.teams[result_uuid] = team

        await self.send_updated_info()
        return result_uuid

    async def remove_team(self, name: str) -> None:
        """
        :param name: Имя команды, которую кикают (должно быть уникально, иначе удалим первую)
        """
        for uuid, team in list(self.teams.items()):
            if team.name == name:
                self.teams.pop(uuid)
                self.black_list.add(uuid)
                await self.send_updated_info()
                break

    def get_team_info(self, uuid: str | None) -> dict[str, Any] | None:
        """
        :param uuid: UUID команды
        :return: Если команда найдена, то имя и очки команды
        """
        if not uuid or uuid not in self.teams:
            return None
        team = self.teams[uuid]
        return {'name': team.name, 'score': team.score}

    def update_score_by_name(self, name: str, score: int) -> None:
        for team in self.teams.values():
            if team.name == name:
                team.score += score
                return

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
            'kicked': self.kicked_uuids,
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
            except Exception:
                dead.append(ws)
        for ws in dead:
            self.sockets.discard(ws)
