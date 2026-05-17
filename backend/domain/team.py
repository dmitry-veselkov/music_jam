from services.crypto import Crypto


class Team:
    def __init__(self, name: str = '', score: int = 0) -> None:
        self.uuid = Crypto.get_uuid()
        self.name = name
        self.score = score
