from typing import Any


class Parser:
    @staticmethod
    def parse_room_info(room_info: list[dict[Any, Any]], tracks_info, teams) -> dict[str, Any]:
        first = room_info[0]
        return {
            'id': first['id'],
            'code': first['join_code'],
            'status': first['status'],
            'title': first['title'],
            'author': first['name'],
            'mode': first['mode'],
            'costs': tracks_info['costs'],
            'categories': tracks_info['categories'],
            'tracks': tracks_info['tracks'],
            'teams': teams,
        }
