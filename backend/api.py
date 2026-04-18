from fastapi import APIRouter, HTTPException, Response, status, Cookie
from pydantic import BaseModel

from typing import Any
from logging import Logger

from services import Services
from db.hands_db import DatabaseHands


class LoginSchema(BaseModel):
    email: str
    password: str


class RegisterSchema(BaseModel):
    email: str
    name: str
    password: str


class TeamSchema(BaseModel):
    id: int
    uuid: str
    name: str


class SaveGameSchema(BaseModel):
    roomCode: str
    name: str = ""
    author: str = ""
    description: str = ""
    maxTeams: int = 4
    categories: list[str] = []
    costs: list[int] = []
    tracks: dict[str, Any] = {}


class ApiRouter:
    def __init__(self, db_hands: DatabaseHands, services: Services, logger: Logger):
        self.router = APIRouter()
        self.db_hands = db_hands
        self.services = services
        self._setup_routes()
        self.logger = logger

    def _setup_routes(self):
        @self.router.post("/login")
        async def login(data: LoginSchema, response: Response):
            pass_hash = self.services.get_hex_hash(data.password)
            user = await self.db_hands.get_user(data.email)

            if not user or user[2] != pass_hash:
                raise HTTPException(
                    status_code=status.HTTP_401_UNAUTHORIZED,
                    detail="Invalid credentials"
                )

            _id, _name, _pass_hash, _email = user
            token = self.services.get_jwt_token(_id, _name, _email)
            response.set_cookie("token", token, httponly=True, samesite="lax")

            return {"message": "Success enter"}

        @self.router.post("/register")
        async def register(data: RegisterSchema, response: Response):
            user = await self.db_hands.get_user(data.email)
            if user:
                raise HTTPException(
                    status_code=status.HTTP_400_BAD_REQUEST,
                    detail="A user with this email already exists"
                )

            pass_hash = self.services.get_hex_hash(data.password)
            _id = await self.db_hands.insert_user(data.name, pass_hash, data.email)

            token = self.services.get_jwt_token(_id, data.name, data.email)
            response.set_cookie("token", token, httponly=True, samesite="lax")

            return {"name": data.name, "email": data.email}

        @self.router.get("/get_all_user_games")
        async def get_all_user_games(token: str | None = Cookie(None)):
            payload = self._get_token_payload(token)
            if not payload:
                raise HTTPException(
                    status_code=status.HTTP_401_UNAUTHORIZED,
                    detail="Invalid credentials"
                )
            _id = int(payload['sub'])
            user_games = await self.db_hands.get_all_user_games(_id)

            for game in user_games:
                game['scheduled_at'] = game['scheduled_at'].strftime("%Y-%m-%d %H:%M:%S")

            return {"games": user_games}

        @self.router.get('/get_user_info')
        async def get_user_info(token: str | None = Cookie(None)):
            self.logger.info(f"Что-то ищем")
            payload = self._get_token_payload(token)
            if not payload:
                raise HTTPException(
                    status_code=status.HTTP_401_UNAUTHORIZED,
                    detail="Invalid credentials"
                )

            return {"email": payload["email"], "name": payload['name']}

        @self.router.get('/get_new_game_code')
        async def create_new_game():
            max_times = 0
            while max_times < 10:
                code = self.services.generate_new_code()
                print(code)
                game_info = await self.db_hands.get_game_info(code)
                if game_info is None:
                    return {"code": code}
                max_times += 1

            raise HTTPException(
                status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
                detail="Could not generate unique code. Try again later."
            )

        @self.router.get('/gameSettings')
        async def get_room_settings(code: str = ''):
            code = code.upper().strip()
            game = await self.db_hands.get_game_info(code)
            if not game:
                return {"exists": False}
            return {"exists": True, "roomCode": code, **game}

        @self.router.post('/gameSettings')
        async def get_room_settings(data: SaveGameSchema):
            code = data.roomCode.upper().strip()
            if not code:
                raise HTTPException(
                    status_code=status.HTTP_400_BAD_REQUEST,
                    detail="roomCode is required"
                )

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

            return {
                "success": True,
                "roomCode": code,
                "game": data
            }

        @self.router.get('/get_room_info')
        async def get_room_info(code: str):
            code = code.upper().strip()
            room_info = await self.db_hands.get_room_info(code)

            if not room_info:
                return None

            first = room_info[0]
            title, author, _status, _id = first['title'], first['name'], first['status'], first['game_id']
            teams = [f['team_name'] for f in room_info]

            return {
                'id': _id,
                'status': _status,
                'title': title,
                'author': author,
                'teams': teams
            }

        @self.router.post('/set_team_name')
        async def set_team_name(data: TeamSchema):
            is_new = await self.db_hands.insert_or_update_team(data.id, data.uuid, data.name)
            return {'status': 'ok', 'new': is_new}

        @self.router.get('/get_team_name')
        async def get_team_name(uuid: str) -> Response:
            team_name = await self.db_hands.get_team_name(uuid)
            return {"name": team_name}

    def _get_token_payload(self, token):
        if not token:
            return None

        payload = self.services.try_get_jwt_payload(token)
        if not payload:
            return None

        return payload
