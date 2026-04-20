from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    PUBLIC_KEY: str
    SECRET_KEY: str
    DB_HOST: str
    DB_PORT: int
    DB_USER: str
    DB_PASS: str
    DB_NAME: str
    SECRET_JWT: str

    model_config = SettingsConfigDict(
        env_file="../.env",
        extra="ignore"
    )

    @property
    def db_url(self):
        return (
            f"postgresql+asyncpg://"
            f"{self.DB_USER}:{self.DB_PASS}"
            f"@{self.DB_HOST}:{self.DB_PORT}/{self.DB_NAME}"
        )
