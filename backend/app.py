import pathlib
from typing import Any

from flask import Flask, Response, send_from_directory

from api import Api
from settings import Settings
from db.database import Database
from db.hands_db import DatabaseHands
from services import Services


class App:
    BASE_DIR = pathlib.Path(__file__).parent.parent
    BACKEND_DIR = BASE_DIR / 'backend'
    FRONTEND_DIR = BASE_DIR / 'frontend'

    def __init__(self) -> None:
        self.app = Flask(__name__, static_folder=str(self.FRONTEND_DIR))
        self.settings = Settings()
        self.db = Database(self.settings)
        self.db_hands = DatabaseHands(self.db)
        self.services = Services(self.settings.SECRET_JWT)

        self.api = Api(self.app, self.db_hands, self.services)
        self._initialize_url_rules()

    def _initialize_url_rules(self) -> None:
        self.app.add_url_rule('/', view_func=self.index, defaults={'path': ''})
        self.app.add_url_rule('/<path:path>', view_func=self.index)

        self._initialize_auth_urls()
        self._initialize_game_urls()

    def _initialize_auth_urls(self):
        self.app.add_url_rule('/api/login', view_func=self.api.login, methods=['POST'])
        self.app.add_url_rule('/api/register', view_func=self.api.register, methods=['POST'])
        self.app.add_url_rule('/api/get_user_info', view_func=self.api.get_user_info, methods=['GET'])
        self.app.add_url_rule('/api/get_all_user_games', view_func=self.api.get_all_user_games, methods=['GET'])

    def _initialize_game_urls(self):
        self.app.add_url_rule('/api/check_room', view_func=self.api.get_room_info, methods=['GET'])
        self.app.add_url_rule('/api/set_team_name', view_func=self.api.set_team_name, methods=['POST'])
        # сделать бы на GET обычный, но тут хз
        self.app.add_url_rule('/api/get_team_name', view_func=self.api.get_team_name, methods=['POST'])

    def run(self, **kwargs: Any) -> None:
        self.app.run(**kwargs)

    def index(self, path: str) -> Response:
        if path != '' and pathlib.Path(str(self.app.static_folder)).joinpath(path).exists():
            return send_from_directory(str(self.app.static_folder), path)
        return send_from_directory(str(self.app.static_folder), 'index.html')


if __name__ == "__main__":
    App().run(debug=True, port=5000)
