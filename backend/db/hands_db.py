from sqlalchemy import text
import asyncio


class DatabaseHands:
    _ALLOWED_GAME_COLUMNS = {"admin_user_id", "title", "join_code", "scheduled_at", "status"}

    def __init__(self, database) -> None:
        self.db = database;

    async def get_user(self, email):
        async with self.db.get_session() as session:
            result = await session.execute(
                text("SELECT * FROM users WHERE email = :email"),
                {"email": email}
            )
            return result.fetchone()

    async def insert_user(self, name, hash_password, email):
        async with self.db.get_session() as session:
            result = await session.execute(
                text("INSERT INTO users (name, password_hash, email) "
                     "VALUES(:name, :hash_password, :email) "
                     "RETURNING id"),
                {
                    "name": name,
                    "hash_password": hash_password,
                    "email": email,
                }
            )

            await session.commit()
            return result.scalar()

    async def get_all_user_games(self, _id):
        async with self.db.get_session() as session:
            result = await session.execute(
                text("""
                     SELECT id, title, join_code, scheduled_at
                     FROM games
                     WHERE admin_user_id = :admin_id
                     """),
                {"admin_id": _id})
            return [dict(r) for r in result.mappings().all()]

    async def get_game_info(self, code):
        async with self.db.get_session() as session:
            result = await session.execute(
                text("SELECT * FROM games WHERE join_code = :join_code"),
                {
                    "join_code": code
                }
            )
            row = result.mappings().first()
        return dict(row) if row else None

    async def get_room_info(self, code):
        print(code)
        async with self.db.get_session() as session:
            result = await session.execute(
                text("SELECT title, name, join_code, status, team_name, score, game_id "
                     "FROM games as g "
                     "LEFT JOIN game_participants AS gp ON g.id = gp.game_id "
                     "JOIN users AS u ON g.admin_user_id = u.id "
                     "WHERE join_code = :join_code"),
                {
                    "join_code": code
                }
            )
            return [dict(r) for r in result.mappings().all()]

    async def get_room_tracks(self, code):
        async with self.db.get_session() as session:
            game_id = (await session.execute(
                text("SELECT id FROM games WHERE join_code = :join_code"),
                {
                    "join_code": code
                }
            )).scalar()

            result = (await session.execute(
                text("SELECT * "
                     "FROM songs as s "
                     "JOIN game_songs AS gs ON s.id = gs.song_id "
                     "WHERE gs.game_id = :game_id"),
                {
                    "game_id": game_id
                }
            )).mappings().all()

            categories = list({r["category"] for r in result})
            costs = sorted(list({r["price"] for r in result}))
            tracks = {
                category: {
                    row["price"]: row["file_url"]
                    for row in result if row["category"] == category
                }
                for category in categories
            }
        return {
            "categories": categories,
            "costs": costs,
            "tracks": tracks
        }

    async def create_game(self, admin_id, title, join_code, scheduled_at):
        async with self.db.get_session() as session:
            await session.execute(
                text("INSERT INTO games (admin_user_id, title, join_code, scheduled_at) "
                     "VALUES(:admin_id, :title, :join_code, :scheduled_at)"),
                {
                    "admin_id": admin_id,
                    "title": title,
                    "join_code": join_code,
                    "scheduled_at": scheduled_at
                }
            )
            await session.commit()

    async def insert_or_update_team(self, game_id, team_id, team_name):
        async with self.db.get_session() as session:
            result = await session.execute(
                text("INSERT INTO game_participants (game_id, team_id, team_name, score) "
                     "VALUES (:game_id, :team_id, :team_name, 0) "
                     "ON CONFLICT (game_id, team_id) "
                     "DO UPDATE SET team_name = EXCLUDED.team_name "
                     "RETURNING xmax"),
                {
                    "game_id": game_id,
                    "team_id": team_id,
                    "team_name": team_name,
                }
            )

            status = result.scalar_one()
            await session.commit()
            return status == 0

    async def get_team_name(self, team_id):
        async with self.db.get_session() as session:
            result = await session.execute(
                text("SELECT team_name "
                     "FROM game_participants "
                     "WHERE team_id = :team_id "),
                {
                    "team_id": team_id,
                }
            )

            return result.scalar_one_or_none()

    async def update_score_team(self, game_id, nickname, add_score):
        # TODO: пока хз по нику или по айди
        session = await self.db.get_session()
        await session.execute(
            text("UPDATE game_participants "
                 "SET score = score + :add_score "
                 "WHERE game_id = :game_id AND nickname = :nickname"),
            {
                "game_id": game_id,
                "nickname": nickname,
                "add_score": add_score
            }
        )
        await session.commit()
        await session.close()

    async def update_game_any_param(self, join_code, column, value):
        if column not in self._ALLOWED_GAME_COLUMNS:
            return
        async with self.db.get_session() as session:
            print(join_code, "any params")
            await session.execute(
                text("UPDATE games "
                     f"SET {column} = :value "
                     "WHERE join_code = :join_code"),
                {
                    "value": value,
                    "join_code": join_code
                }
            )
            await session.commit()

    async def update_game_settings(self, join_code, songs):
        categories = songs.get("categories", [])
        costs = songs.get("costs", [])
        tracks = songs.get("tracks", {})
        actual_song_ids = []
        async with self.db.get_session() as session:
            game_id = (await session.execute(
                text("SELECT id FROM games WHERE join_code = :join_code"),
                {
                    "join_code": join_code
                }
            )).scalar()
            print(join_code, "game settings")
            for category in categories:
                for cost in costs:
                    value = tracks.get(category, {}).get(cost)
                    song_id = (await session.execute(
                         text("INSERT INTO songs (category, price, file_url) "
                             "VALUES (:category, :price, :file_url) "
                             "RETURNING id"),
                        {
                            "category": category,
                            "price": cost,
                            "file_url": value,
                        }
                    )).scalar()
                    actual_song_ids.append(song_id)
                    await session.execute(
                        text("INSERT INTO game_songs (game_id, song_id) "
                             "VALUES (:game_id, :song_id) "
                             "ON CONFLICT (game_id, song_id) "
                             "DO UPDATE SET song_id = EXCLUDED.song_id "
                             "RETURNING id"),
                        {
                            "song_id": song_id,
                            "game_id": game_id,
                        }
                    )
            if actual_song_ids:
                await session.execute(
                    text("DELETE FROM game_songs WHERE game_id = :game_id "
                         "AND song_id != ALL(:song_ids)"),
                    {
                        "game_id": game_id,
                        "song_ids": list(actual_song_ids)
                    }
                )
            else:
                await session.execute(
                    text("DELETE FROM game_songs WHERE game_id = :game_id"),
                    {
                        "game_id": game_id
                    }
                )
            await session.commit()



