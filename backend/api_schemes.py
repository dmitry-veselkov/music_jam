from typing import Any

from pydantic import BaseModel


class LoginSchema(BaseModel):
    email: str
    password: str


class RegisterSchema(BaseModel):
    email: str
    name: str
    password: str


class TeamSchema(BaseModel):
    id: int
    code: str
    uuid: str
    oldName: str
    name: str


class SaveGameSchema(BaseModel):
    roomCode: str
    name: str = ""
    author: str = ""
    description: str = ""
    maxTeams: int = 4
    categories: list[str] = []
    costs: list[int] = []
    tracks: dict[str, Any] = {}
