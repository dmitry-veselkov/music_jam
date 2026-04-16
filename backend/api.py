from flask import Flask, Response, jsonify, request


class Api:
    def __init__(self, app: Flask) -> None:
        self.app = app
        self.teams = {"fake-uuid-1": "БобрДобр", "fake-uuid-2": "Недавно гипербола"}
        # TODO избавиться от self.teams как простой dict
        # мб передавать БД в качестве аргумента для запросов

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
