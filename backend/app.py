import pathlib
from typing import Any

from fastapi import FastAPI, Request
from fastapi.staticfiles import StaticFiles
from fastapi.responses import FileResponse
from fastapi.middleware.cors import CORSMiddleware

import uvicorn
import logging

from api import ApiRouter
from settings import Settings
from db.database import Database
from db.hands_db import DatabaseHands
from services import Services

class App:
    BASE_DIR = pathlib.Path(__file__).parent.parent
    BACKEND_DIR = BASE_DIR / 'backend'
    FRONTEND_DIR = BASE_DIR / 'frontend'

    def __init__(self) -> None:
        self.app = FastAPI(title="Music Jam API")
        self.settings = Settings()

        self.db = Database(self.settings)
        self.db_hands = DatabaseHands(self.db)
        self.services = Services(self.settings.SECRET_JWT)
        self.logger = logging.getLogger("uvicorn")

        self.app.add_middleware(
            CORSMiddleware,
            allow_origins=["*"],
            allow_methods=["*"],
            allow_headers=["*"],
        )

        self._initialize_routers()
        self._initialize_static()

    def _initialize_routers(self):
        api_router = ApiRouter(self.db_hands, self.services, self.logger)
        self.app.include_router(api_router.router, prefix="/api")

    def _initialize_static(self):
        if self.FRONTEND_DIR.exists():
            self.app.mount("/static", StaticFiles(directory=str(self.FRONTEND_DIR)), name="static")
            @self.app.get("/{path_name:path}")
            async def catch_all(request: Request, path_name: str):
                file_path = self.FRONTEND_DIR / path_name
                if file_path.is_file():
                    return FileResponse(file_path)
                return FileResponse(self.FRONTEND_DIR / "index.html")

    def run(self):
        uvicorn.run(self.app, host="127.0.0.1", port=5000)

    # def _initialize_game_urls(self):
    #     self.app.add_url_rule('/api/check_room', view_func=self.api.get_room_info, methods=['GET'])
    #     self.app.add_url_rule('/api/set_team_name', view_func=self.api.set_team_name, methods=['POST'])
    #     # сделать бы на GET обычный, но тут хз
    #     self.app.add_url_rule('/api/get_team_name', view_func=self.api.get_team_name, methods=['POST'])


if __name__ == "__main__":
    App().run()
