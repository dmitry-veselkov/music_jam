from typing import Any

from fastapi import Cookie, HTTPException, Response, status
from routes import Routes


class Get:
    def __init__(self, outer: Routes) -> None:
        self.outer = outer

    async def get_all_user_games(self, token: str | None = Cookie(None)) -> list[dict[str, Any]]:
        payload = self.outer.get_token_payload(token)
        if not payload:
            raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")
        _id = int(payload['sub'])
        user_games = await self.outer.db_hands.get_all_user_games(_id)

        for game in user_games:
            game['scheduled_at'] = game['scheduled_at'].strftime("%Y-%m-%d %H:%M:%S")

        return user_games

    async def get_user_info(self, token: str | None = Cookie(None)) -> dict[str, Any]:
        self.outer.logger.info("Что-то ищем")
        payload = self.outer.get_token_payload(token)
        if not payload:
            raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

        return {"email": payload["email"], "name": payload['name']}

    async def create_new_game(self) -> str:
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

    async def get_room_settings(self, code: str = '') -> dict[str, Any]:
        code = code.upper().strip()
        game = await self.outer.db_hands.get_game_info(code)
        if not game:
            return {"exists": False}
        return {"exists": True, "roomCode": code, **game}

    async def get_room_info(self, code: str) -> dict[str, Any] | None:
        code = code.upper().strip()
        room_info = await self.outer.db_hands.get_room_info(code)
        return self.outer.services.parse_room_info(room_info) if room_info else None

    async def get_team_name(self, uuid: str) -> Any | None:
        return await self.outer.db_hands.get_team_name(uuid)

    async def run_game(self, code: str = '') -> dict[str, Any] | None:
        code = code.upper().strip()
        await self.outer.db_hands.update_game_any_param(code, "status", "waiting")
        room_info = await self.outer.db_hands.get_room_info(code)
        return self.outer.services.parse_room_info(room_info) if room_info else None
