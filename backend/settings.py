from pydantic import Field
from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    DB_HOST: str = Field(default="")
    DB_PORT: int = Field(default=0)
    DB_USER: str = Field(default="")
    DB_PASS: str = Field(default="")
    DB_NAME: str = Field(default="")
    SECRET_JWT: str = Field(default="")

    model_config = SettingsConfigDict(env_file="../.env", extra="ignore")

    @property
    def db_url(self) -> str:
        return (
            f"postgresql+asyncpg://" f"{self.DB_USER}:{self.DB_PASS}" f"@{self.DB_HOST}:{self.DB_PORT}/{self.DB_NAME}"
        )
