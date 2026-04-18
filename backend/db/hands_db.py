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
            row = result.fetchall()
            if not row:
                return None
            return row

    async def get_room_info(self, code):
        print(code)
        async with self.db.get_session() as session:
            result = await session.execute(
                text("SELECT title, name, join_code, status, team_name, score, game_id "
                     "FROM games as g "
                     "JOIN game_participants AS gp ON g.id = gp.game_id "
                     "JOIN users AS u ON g.admin_user_id = u.id "
                     "WHERE join_code = :join_code"),
                {
                    "join_code": code
                }
            )
            return [dict(r) for r in result.mappings().all()]

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
