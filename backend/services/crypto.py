import datetime
import hashlib
import secrets
from typing import Any

import jwt
from uuid import uuid4


class Crypto:
    _CODE_ALPHABET = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ"
    _CODE_LENGTH = 6

    def __init__(self, secret_jwt: str) -> None:
        self.secret_jwt = secret_jwt

    @staticmethod
    def get_hex_hash(origin_string: str) -> str:
        hash_machine = hashlib.sha256()
        hash_machine.update(origin_string.encode())
        return hash_machine.hexdigest()

    def get_jwt_token(self, _id: int, name: str, email: str) -> str:
        payload = {
            "sub": str(_id),
            "email": email,
            "name": name,
            "exp": datetime.datetime.now() + datetime.timedelta(days=1),
        }

        return jwt.encode(payload, self.secret_jwt, algorithm="HS256")

    def get_token_payload(self, token: str | None) -> dict[str, Any] | None:
        if not token:
            return None
        try:
            payload = jwt.decode(token, self.secret_jwt, algorithms=["HS256"])
            return payload
        except jwt.PyJWTError as e:
            return None

    def generate_new_code(self) -> str:
        return ''.join(secrets.choice(self._CODE_ALPHABET) for _ in range(self._CODE_LENGTH))

    @staticmethod
    def get_uuid() -> str:
        return str(uuid4())
