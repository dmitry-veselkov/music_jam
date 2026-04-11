from flask import Flask, send_from_directory, jsonify, request
import pathlib

from backend.api import Api


class App:
    BASE_DIR = pathlib.Path(__file__).parent.parent
    BACKEND_DIR = BASE_DIR / 'backend'
    FRONTEND_DIR = BASE_DIR / 'frontend'

    def __init__(self):
        self.app = Flask(__name__, static_folder=str(self.FRONTEND_DIR))
        self.api = Api(self.app)

        self.app.add_url_rule('/', 'index', self.index, defaults={'path': ''})
        self.app.add_url_rule('/<path:path>', 'index', self.index)
        self.app.add_url_rule('/api/check_room', None, self.api.get_room_info, methods=['GET'])
        self.app.add_url_rule('/api/set_team_name', None, self.api.set_team_name, methods=['POST'])
        self.app.add_url_rule('/api/get_team_name', None, self.api.get_team_name, methods=['POST']) # сделать бы на GET обычный, но тут хз

    def run(self, **kwargs):
        self.app.run(**kwargs)

    def index(self, path):
        if path != '' and pathlib.Path(str(self.app.static_folder)).joinpath(path).exists():
            return send_from_directory(str(self.app.static_folder), path)
        return send_from_directory(str(self.app.static_folder), 'index.html')


if __name__ == "__main__":
    App().run(debug=True, port=5000)
