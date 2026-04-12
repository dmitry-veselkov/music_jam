import pathlib
from typing import Any

from flask import Flask, Response, send_from_directory

from backend.api import Api


class App:
    BASE_DIR = pathlib.Path(__file__).parent.parent
    BACKEND_DIR = BASE_DIR / 'backend'
    FRONTEND_DIR = BASE_DIR / 'frontend'

    def __init__(self) -> None:
        self.app = Flask(__name__, static_folder=str(self.FRONTEND_DIR))
        self.api = Api(self.app)

        self.app.add_url_rule('/', view_func=self.index, defaults={'path': ''})
        self.app.add_url_rule('/<path:path>', view_func=self.index)
        self.app.add_url_rule('/api/check_room', view_func=self.api.get_room_info, methods=['GET'])
        self.app.add_url_rule('/api/set_team_name', view_func=self.api.set_team_name, methods=['POST'])
        self.app.add_url_rule('/api/get_team_name', view_func=self.api.get_team_name, methods=['POST'])
        # сделать бы на GET обычный, но тут хз

    def run(self, **kwargs: Any) -> None:
        self.app.run(**kwargs)

    def index(self, path: str) -> Response:
        if path != '' and pathlib.Path(str(self.app.static_folder)).joinpath(path).exists():
            return send_from_directory(str(self.app.static_folder), path)
        return send_from_directory(str(self.app.static_folder), 'index.html')


if __name__ == "__main__":
    App().run(debug=True, port=5000)
