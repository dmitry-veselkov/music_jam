from services import Services


class Team:
    def __init__(self, name: str = '', score: int = 0) -> None:
        self.uuid = Services.get_uuid()
        self.name = name
        self.score = score
