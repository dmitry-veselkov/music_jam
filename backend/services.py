import hashlib
import datetime
import jwt
import secrets


class Services:
    _CODE_ALPHABET = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ"
    _CODE_LENGTH = 6

    def __init__(self, secret_jwt) -> None:
        self.secret_jwt = secret_jwt

    @staticmethod
    def get_hex_hash(origin_string: str) -> str:
        hash_machine = hashlib.sha256()
        hash_machine.update(origin_string.encode())
        return hash_machine.hexdigest()

    def get_jwt_token(self, _id: int, name: str, email: str):
        payload = {
            "sub": str(_id),
            "email": email,
            "name": name,
            "exp": datetime.datetime.now() + datetime.timedelta(minutes=15)
        }

        return jwt.encode(payload, self.secret_jwt, algorithm="HS256")

    def try_get_jwt_payload(self, token):
        try:
            payload = jwt.decode(token, self.secret_jwt, algorithms=["HS256"])
            return payload
        except jwt.PyJWTError as e:
            print(e)
            return None

    def generate_new_code(self):
        return ''.join(secrets.choice(self._CODE_ALPHABET) for _ in range(self._CODE_LENGTH))

    @staticmethod
    def parse_room_info(room_info):
        first = room_info[0]
        teams = [f['team_name'] for f in room_info]

        return {
            'id': first['game_id'],
            'code': first['join_code'],
            'status': first['status'],
            'title': first['title'],
            'author': first['name'],
            'teams': teams
        }
