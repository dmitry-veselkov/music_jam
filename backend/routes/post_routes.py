from typing import Any

from fastapi import HTTPException, Response, status

from api_schemes import LoginSchema, RegisterSchema, SaveGameSchema, TeamSchema
from routes import Routes


class Post:
    def __init__(self, outer: Routes) -> None:
        self.outer = outer

    async def login(self, data: LoginSchema, response: Response) -> bool:
        pass_hash = self.outer.services.get_hex_hash(data.password)
        user = await self.outer.db_hands.get_user(data.email)

        if not user or user[2] != pass_hash:
            raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

        _id, _name, _pass_hash, _email = user
        token = self.outer.services.get_jwt_token(_id, _name, _email)
        response.set_cookie("token", token, httponly=True, samesite="lax")
        return True

    async def register(self, data: RegisterSchema, response: Response) -> dict[str, str]:
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

    @staticmethod
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

    async def set_team_name(self, data: TeamSchema) -> dict[str, Any]:
        is_new = await self.outer.db_hands.insert_or_update_team(data.id, data.uuid, data.name)
        code = data.code
        await self.outer.ensure_room_loaded(code)

        teams = self.outer.room_state[code]["teams"]

        if is_new:
            if data.name not in teams:
                teams.append(data.name)
        else:
            for i, t in enumerate(teams):
                if t == data.oldName:
                    teams[i] = data.name
                    break

        await self.outer.broadcast_room(code)
        return {"status": "ok", "new": is_new}
