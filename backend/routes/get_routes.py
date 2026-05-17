from typing import Any, TYPE_CHECKING

from fastapi import Cookie, HTTPException, Response, status
from fastapi.responses import StreamingResponse
from domain.room import Room

if TYPE_CHECKING:
    from routes.routes import Routes


class Get:
    """
    Обработчик GET-запросов на сервер
    """

    def __init__(self, outer: "Routes") -> None:
        self.outer = outer
        self.crypto = self.outer.services.crypto
        self.parser = self.outer.services.parser

    async def get_all_user_games(self, token: str | None = Cookie(None)) -> list[dict[str, Any]]:
        """
        :param token: JWT-токен пользователя из Cookies
        :return: Список всех игр данного пользователя
        """
        payload = self.crypto.get_token_payload(token)
        if not payload:
            raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")
        _id = int(payload['sub'])
        user_games = await self.outer.db_hands.get_all_user_games(_id)

        return user_games

    async def get_user_info(self, token: str | None = Cookie(None)) -> dict[str, Any]:
        """
        :param token: JWT-токен пользователя из Cookies
        :return: Информация о пользователе: email, name, id
        """
        payload = self.crypto.get_token_payload(token)
        if not payload:
            raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

        return {"email": payload["email"], "name": payload['name'], 'id': payload['sub']}

    # TODO: это должно быть про game, ибо про сокеты здесь ничего нет и к rooms не обращаемся
    # TODO: изменить url апишки
    # TODO: exists нафиг не нужен, можем просто None возвращать
    # TODO: после правок дописать доку к методу
    async def get_game_settings(self, code: str = '') -> dict[str, Any] | None:
        code = code.upper().strip()
        game = await self.outer.db_hands.get_game_info(code)
        if not game:
            return {"exists": False}
        songs = await self.outer.db_hands.get_room_tracks(code)

        return {"exists": True, "roomCode": code, **game, **songs}

    # TODO: переписать получение game на gameId, code в БД не должен лежать...
    # TODO: после правок дописать доку к методу
    async def get_room_info(self, code: str) -> dict[str, Any] | None:
        code = code.upper().strip()
        room_info = await self.outer.db_hands.get_room_info(code)
        songs = await self.outer.db_hands.get_room_tracks(code)
        teams = None
        if code in self.outer.rooms:
            teams = self.outer.rooms[code].team_names
        return self.parser.parse_room_info(room_info, songs, teams) if room_info else None

    # TODO: зачем возвращать такой словарь, если можно вернуть None
    async def get_room_state(self, code: str) -> dict[str, Any] | None:
        code = code.upper().strip()
        room = self.outer.rooms.get(code)
        if not room:
            return {"teams": {}, "played_tracks": []}
        return {
            "teams": room.teams_scores,
            "played_tracks": [list(t) for t in room.played_tracks]
        }

    async def get_team_info(self, code: str, team_uuid: str | None = Cookie(None)) -> dict[str, Any] | None:
        """
        :param code: Код комнаты
        :param team_uuid: UUID конкректной команды из Cookie
        :return: Информация о команде: название и ее очки
        """
        code = code.upper().strip()
        room = self.outer.rooms.get(code)
        if not room or not team_uuid:
            return None
        return room.get_team_info(team_uuid)

    async def check_kicked(self, code: str, response: Response, team_uuid: str | None = Cookie(None)) -> bool:
        """
        :param code: Код комнаты
        :param response: Response для изменения куки
        :param team_uuid: UUID конкректной команды из Cookie
        :return: была удалена команда или нет
        """
        code = code.upper().strip()
        room = self.outer.rooms.get(code)
        if not room:
            return False
        is_kicked = room.is_team_kicked(team_uuid)
        if is_kicked:
            response.delete_cookie('team_uuid')
        return is_kicked

    async def start_room(self, code: str) -> None:
        """
        :param code: Код комнаты
        """
        code = code.upper().strip()
        if code in self.outer.rooms:
            return
        self.outer.rooms[code] = Room(code)
        await self.outer.db_hands.update_game_any_param(code, "status", "waiting")

    async def play_song(self, file_name: str) -> StreamingResponse:
        """
        :param file_name: Имя музыкального файла для воспроизведения
        :return: Stream с передачей нужно файла
        """
        body, content_type = self.outer.services.s3.get_stream(file_name)
        return StreamingResponse(
            body.iter_chunks(chunk_size=1024 * 512),
            media_type=content_type)
