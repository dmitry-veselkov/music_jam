from services.crypto import Crypto
from services.parser import Parser
from services.s3 import S3


class Services:
    def __init__(self, secret_jwt: str, public_key: str, secret_key: str) -> None:
        self.crypto = Crypto(secret_jwt)
        self.parser = Parser()
        self.s3 = S3(public_key, secret_key)
