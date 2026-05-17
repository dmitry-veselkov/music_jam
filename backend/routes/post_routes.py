from typing import Any, TYPE_CHECKING

from fastapi import (
    HTTPException,
    Response,
    status,
    Cookie,
    Form,
    UploadFile,
    File
)

from routes.schemes import (
    LoginSchema,
    RegisterSchema,
    SaveGameSchema,
    TeamSchema,
    AddPointsSchema,
    AddPlayedTrack,
    KickTeamSchema
)

if TYPE_CHECKING:
    from routes.routes import Routes


class Post:
    """
    Обработчик POST-запросов на сервер
    """

    def __init__(self, outer: "Routes") -> None:
        self.outer = outer
        self.crypto = self.outer.services.crypto
        self.s3 = self.outer.services.s3

    async def login(self, data: LoginSchema, response: Response) -> bool:
        pass_hash = self.crypto.get_hex_hash(data.password)
        user = await self.outer.db_hands.get_user(data.email)

        if not user or user[2] != pass_hash:
            raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

        _id, _name, _pass_hash, _email = user
        token = self.crypto.get_jwt_token(_id, _name, _email)
        response.set_cookie("token", token, httponly=True, samesite="lax")

        return True

    async def register(self, data: RegisterSchema, response: Response) -> dict[str, str]:
        user = await self.outer.db_hands.get_user(data.email)
        if user:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST, detail="A user with this email already exists"
            )

        pass_hash = self.crypto.get_hex_hash(data.password)
        _id = await self.outer.db_hands.insert_user(data.name, pass_hash, data.email)

        token = self.crypto.get_jwt_token(_id, data.name, data.email)
        response.set_cookie("token", token, httponly=True, samesite="lax")

        return {"name": data.name, "email": data.email}

    async def logout(self, response: Response) -> None:
        response.delete_cookie("token")

    async def create_new_game(self, token: str = Cookie(None)):
        payload = self.crypto.get_jwt_payload(token)
        _id = int(payload["sub"])
        for max_times in range(10):
            code = self.crypto.generate_new_code()
            game_info = await self.outer.db_hands.get_game_info(code)
            if game_info is None:
                await self.outer.db_hands.create_game(
                    _id,
                    "Новая игра",
                    '',
                    code,
                )
                return code
        raise HTTPException(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            detail="Could not generate unique code. Try again later.",
        )

    async def save_room_settings(self, data: SaveGameSchema):
        code = data.roomCode.upper().strip()
        if not code:
            raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="roomCode is required")

        await self.outer.db_hands.update_game_any_param(code, "title", data.title)
        await self.outer.db_hands.update_game_any_param(code, "description", data.description)
        await self.outer.db_hands.update_game_any_param(code, "mode", data.mode)

        await self.outer.db_hands.save_game_grid(
            code,
            data.categories,
            data.costs,
            data.tracks
        )

        return {
            "success": True,
            "roomCode": code
        }

    async def give_points(self, data: AddPointsSchema):
        code = data.code.upper().strip()
        room = self.outer.rooms.get(code)
        if room:
            room.update_score_by_name(data.team, data.points)
        return {"success": True}

    async def update_team_name(self,
                               data: TeamSchema,
                               response: Response,
                               team_uuid: str | None = Cookie(None)
                               ) -> str | None:
        """
        :param data: Информация о команде (код комнаты и новое название)
        :param response: Ответ для сохранения куки
        :param team_uuid: UUID команды (из Cookie)
        """
        code = data.code.upper().strip()
        room = self.outer.rooms.get(code)
        if not room:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Room not found")

        new_uuid = await room.update_team_name(uuid=team_uuid, new_name=data.name)
        if new_uuid is not None:
            response.set_cookie("team_uuid", new_uuid, httponly=True, samesite="lax")
        return new_uuid

    async def remove_team(self, data: KickTeamSchema) -> None:
        code = data.code.upper().strip()
        room = self.outer.rooms.get(code)
        if room:
            await room.remove_team(data.name)

    async def delete_game(self, code: str = '') -> None:
        code = code.upper().strip()
        await self.outer.db_hands.delete_game(code)

    async def start_game(self, code: str = '') -> dict[str, bool]:
        code = code.upper().strip()
        await self.outer.db_hands.update_game_any_param(code, "status", "playing")
        await self.outer.rooms[code].broadcast_start()
        return {"success": True}

    async def end_game(self, code: str = '') -> dict[str, Any]:
        code = code.upper().strip()
        await self.outer.db_hands.update_game_any_param(code, "status", "ended")
        room = self.outer.rooms.get(code)
        final_players: dict[str, int] = {}
        if room:
            final_players = room.teams_scores
            await room.send_payload_to_all({
                "type": "game_ended",
                "players": final_players,
            })
            room.close_room()
            self.outer.rooms.pop(code, None)
        return {"success": True, "players": final_players}

    async def add_played_track(self, data: AddPlayedTrack):
        code = data.code.upper().strip()
        self.outer.rooms[code].add_played_track(data.row, data.column)
        return {"success": True}

    async def upload_track(self,
                           title: str = Form(...),
                           artist: str = Form(...),
                           category: str = Form(...),
                           cost: str = Form(...),
                           question: UploadFile = File(None),
                           answer: UploadFile = File(None),
                           question_url: str = Form(None),
                           answer_url: str = Form(None)):

        if not title.strip() or not artist.strip():
            return {"error": "Не все поля заполнены"}

        if question:
            if not question.content_type.startswith("audio/"):
                return {"error": "Файл вопроса не аудио!"}
            question_name = self.crypto.get_unique_s3_uuid(question.filename)
            self.outer.services.s3.upload(question.file, question_name, question.content_type)
        elif question_url:
            question_name = question_url
        else:
            return {"error": "Нет файла или ссылки для вопроса"}

        if answer:
            if not answer.content_type.startswith("audio/"):
                return {"error": "Файл вопроса не аудио!"}
            answer_name = self.s3.get_unique_s3_uuid(answer.filename)
            self.outer.services.s3.upload(answer.file, answer_name, answer.content_type)
        elif answer_url:
            answer_name = answer_url
        else:
            return {"error": "Нет файла или ссылки для ответа"}

        return {
            "status": "ok",
            "song": {
                "title": title,
                "artist": artist,
                "questionUrl": question_name,
                "answerUrl": answer_name
            }
        }
