from pydantic import BaseModel
from typing import Any


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
    title: str
    description: str
    categories: list[str]
    costs: list[int]
    tracks: list[list[dict | None]]

class AddPointsSchema(BaseModel):
    points : int
    team : str
    correct : bool
