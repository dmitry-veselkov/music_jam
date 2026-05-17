import logging
import pathlib
from typing import Any
import uvicorn
from routes.routes import Routes
from db.database import Database
from db.hands_db import DatabaseHands
from fastapi import FastAPI, Request
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import FileResponse
from fastapi.staticfiles import StaticFiles
from services.services import Services
from settings import Settings


class App:
    BASE_DIR = pathlib.Path(__file__).parent.parent
    BACKEND_DIR = BASE_DIR / 'backend'
    FRONTEND_DIR = BASE_DIR / 'frontend'

    def __init__(self) -> None:
        self.app = FastAPI(title="Music Jam API")
        self.settings = Settings()

        self.db = Database(self.settings)
        self.db_hands = DatabaseHands(self.db)
        self.services = Services(self.settings.SECRET_JWT, self.settings.PUBLIC_KEY, self.settings.SECRET_KEY)
        self.logger = logging.getLogger("uvicorn")

        self.app.add_middleware(
            CORSMiddleware,
            allow_origins=["*"],
            allow_methods=["*"],
            allow_headers=["*"],
        )

        self._initialize_routers()
        self._initialize_static()

    def _initialize_routers(self) -> None:
        # TODO по идее api не должно ничего знать про S3. это хороший функционал для сервиса
        # TODO который потом уже можно будет разбить на специализированные подклассы: криптография, s3 и пр.
        api_router = Routes(self.db_hands, self.services, self.logger)
        self.app.include_router(api_router.router, prefix='/api')

    def _initialize_static(self) -> None:
        if self.FRONTEND_DIR.exists():
            self.app.mount("/static", StaticFiles(directory=str(self.FRONTEND_DIR)), name="static")
            self.app.add_api_route('/{path_name:path}', self.catch_all, methods=["GET"])

    async def catch_all(self, request: Request, path_name: str) -> FileResponse:  # а зачем тут request?
        # TODO: действительно, хороший вопрос: а зачем request?
        file_path = self.FRONTEND_DIR / path_name
        if file_path.is_file():
            return FileResponse(file_path)
        return FileResponse(self.FRONTEND_DIR / "index.html")

    def run(self, host: str = "127.0.0.1", port: int = 5000, **kwargs: Any) -> None:
        uvicorn.run(self.app, host=host, port=port, **kwargs)


if __name__ == "__main__":
    App().run()
