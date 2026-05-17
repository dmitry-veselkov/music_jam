from logging import Logger
from db.hands_db import DatabaseHands
from fastapi import APIRouter
from routes.get_routes import Get
from routes.post_routes import Post
from services.services import Services
from routes.ws_routes import WS
from domain.room import Room


class Routes:
    def __init__(self, db_hands: DatabaseHands, services: Services, logger: Logger) -> None:
        self.router = APIRouter()
        self.db_hands = db_hands
        self.services = services
        self.logger = logger

        self.get_routes = Get(self)
        self.post_routes = Post(self)
        self.ws_routes = WS(self)

        self._setup_routes()

        # TODO: пока так, но потом нужно создать нормальную слоистую архитектуру
        # TODO: апи должно ссылаться на слой application, он уже должен вызывать доменку, а она бд
        # TODO: не должен апи ходить в бд... ну, не должен...
        self.rooms: dict[str, Room] = {}

    def _setup_routes(self) -> None:
        for name in dir(self.get_routes):
            if not name.startswith("_") and callable(method := getattr(self.get_routes, name)):
                self.router.get(f'/{name}')(method)

        for name in dir(self.post_routes):
            if not name.startswith("_") and callable(method := getattr(self.post_routes, name)):
                self.router.post(f'/{name}')(method)

        # TODO: ну тут только 1 метод и будет на подключение. остальное без апишки...
        for name in dir(self.ws_routes):
            if not name.startswith("_") and callable(method := getattr(self.ws_routes, name)):
                print('ws')
                self.router.websocket(f'/ws/room/' + '{code}')(method)
