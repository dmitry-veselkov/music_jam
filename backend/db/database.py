from sqlalchemy.ext.asyncio import create_async_engine, AsyncSession, async_sessionmaker
from sqlalchemy.orm import sessionmaker


class Database:
    def __init__(self, settings) -> None:
        engine = create_async_engine(settings.db_url, echo=True)
        self.SessionLocal = sessionmaker(engine, class_=AsyncSession, expire_on_commit=False)

    def get_session(self) -> AsyncSession:
        return self.SessionLocal()
