from settings import Settings
from sqlalchemy.ext.asyncio import AsyncSession, async_sessionmaker, create_async_engine


class Database:
    def __init__(self, settings: Settings) -> None:
        engine = create_async_engine(settings.db_url, echo=True)
        # self.SessionLocal = sessionmaker(engine, class_=AsyncSession, expire_on_commit=False)
        self.SessionLocal = async_sessionmaker(
            bind=engine,
            class_=AsyncSession,
            expire_on_commit=False,
        )

    def get_session(self) -> AsyncSession:
        return self.SessionLocal()
