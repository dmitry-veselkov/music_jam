from pydantic_settings import BaseSettings, SettingsConfigDict
from sqlalchemy.ext.asyncio import AsyncSession, async_sessionmaker, create_async_engine
from sqlalchemy.orm import DeclarativeBase


class Settings(BaseSettings):
    DB_HOST: str
    DB_PORT: int
    DB_USER: str
    DB_PASS: str
    DB_NAME: str

    @property
    def DATABASE_URL_asyncpg(self):
        return f"postgresql+asyncpg://{self.DB_USER}:{self.DB_PASS}@{self.DB_HOST}:{self.DB_PORT}/{self.DB_NAME}"
    model_config = SettingsConfigDict(env_file=".env")
settings = Settings()
async_engine = create_async_engine(
    url=settings.DATABASE_URL_asyncpg,
    echo=True,
)
async_session_factory = async_sessionmaker(
    async_engine,
    expire_on_commit=False
)
class Base(DeclarativeBase):
    pass
async def get_db() -> AsyncSession:
    async with async_session_factory() as session:
        yield session