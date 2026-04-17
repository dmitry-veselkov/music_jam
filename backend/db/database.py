from pydantic_settings import BaseSettings, SettingsConfigDict
from sqlalchemy.ext.asyncio import create_async_engine, AsyncSession
from sqlalchemy.orm import sessionmaker


class Settings(BaseSettings):
    DB_HOST: str
    DB_PORT: int
    DB_USER: str
    DB_PASS: str
    DB_NAME: str

    model_config = SettingsConfigDict(
        env_file="../../.env",
        extra="ignore"
    )
    print(model_config)
    @property
    def db_url(self):
        return (
            f"postgresql+asyncpg://"
            f"{self.DB_USER}:{self.DB_PASS}"
            f"@{self.DB_HOST}:{self.DB_PORT}/{self.DB_NAME}"
        )


settings = Settings()

engine = create_async_engine(settings.db_url, echo=True)

SessionLocal = sessionmaker(engine, class_= AsyncSession, expire_on_commit = False)

async def get_session() -> AsyncSession:
    return SessionLocal()