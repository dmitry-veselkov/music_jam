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
    oldName: str
    name: str

class RemoveTeamSchema(BaseModel):
    team_name: str
    code: str

class SaveGameSchema(BaseModel):
    roomCode: str
    title: str
    description: str
    mode : bool
    categories: list[str]
    costs: list[int]
    tracks: list[list[dict | None]]

class AddPointsSchema(BaseModel):
    code: str
    team : str
    points : int

class AddPlayedTrack(BaseModel):
    code : str
    row : int
    column : int
