from flask import Flask, Response, jsonify, request, make_response

from services import Services
from db.hands_db import DatabaseHands


class Api:
    def __init__(self, app: Flask, db_hands: DatabaseHands, services: Services) -> None:
        self.app = app
        self.db_hands = db_hands
        self.services = services
        self.teams = {"fake-uuid-1": "БобрДобр", "fake-uuid-2": "Недавно гипербола"}
        # TODO избавиться от self.teams как простой dict
        # мб передавать БД в качестве аргумента для запросов

    async def login(self) -> Response:
        data = request.get_json()

        email = data.get('email')
        password = data.get('password')
        pass_hash = self.services.get_hex_hash(password)

        user = await self.db_hands.get_user(email)
        if not user or user[2] != pass_hash:
            return jsonify({"message": "Invalid credentials"}), 400

        _id, _name, _pass_hash, _email = user

        token = self.services.get_jwt_token(_id, _name, _email)
        response = make_response()
        response.set_cookie('token', token, httponly=True, samesite='Lax')

        return response, 200

    async def register(self):
        data = request.get_json()

        name = data.get('name')
        email = data.get('email')
        password = data.get('password')

        user = await self.db_hands.get_user(email)
        if user:
            return jsonify({"message": "Пользователь уже существует"}), 400

        pass_hash = self.services.get_hex_hash(password)
        _id = await self.db_hands.insert_user(name, pass_hash, email)

        token = self.services.get_jwt_token(_id, name, email)
        response = make_response(jsonify({
            "user": {
                "name": name,
                "email": email,
            }
        }))

        response.set_cookie('token', token, httponly=True, samesite='Lax')
        return response, 200

    async def get_all_user_games(self):
        payload = self._get_token_payload()
        if not payload:
            jsonify({"message": "Not authenticated"}), 401
        _id = int(payload['sub'])

        user_games = await self.db_hands.get_all_user_games(_id)

        print(user_games)

        for game in user_games:
            game['scheduled_at'] = game['scheduled_at'].strftime("%Y-%m-%d %H:%M:%S")

        return jsonify({"games": user_games}), 200

    def get_user_info(self) -> Response:
        payload = self._get_token_payload()
        if not payload:
            return jsonify({"message": "Not authenticated"}), 401

        return jsonify({
            "email": payload["email"],
            "name": payload['name']
        }), 200

    def _get_token_payload(self):
        token = request.cookies.get('token')
        if not token:
            return None

        payload = self.services.try_get_jwt_payload(token)
        if not payload:
            return None

        return payload

    def get_room_info(self) -> Response:
        code = request.args.get('roomCode')

        # TODO Переделать MOCK
        # TODO Переписать self.teams на запросы к БД
        if code is not None:
            code = code.upper()
            exists = code == "12345"
            room_info = (
                {
                    "status": "waiting",
                    "gameName": "Хиты 80-х",
                    "creator": "Очень добрый человек",
                    "teams": list(self.teams.values()),
                }
                if exists
                else None
            )
        else:
            room_info = None
            exists = False

        return jsonify({"roomCode": code, "exists": exists, "roomInfo": room_info})

    def set_team_name(self) -> Response:
        data = request.get_json()
        team_id = data.get('teamId')

        is_new_team = team_id not in self.teams
        old_name = self.teams.get(team_id)

        self.teams[data.get('teamId')] = data.get('newName')
        return jsonify({"status": "ok", "isNewTeam": is_new_team, "oldName": old_name, "newName": data.get('newName')})

    def get_team_name(self) -> Response:
        data = request.get_json()
        team_name = self.teams.get(data.get('uuid'))
        return jsonify({"teamName": team_name})
