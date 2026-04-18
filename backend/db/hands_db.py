from sqlalchemy import text
import asyncio


class DatabaseHands:
    def __init__(self, database) -> None:
        self.db = database;

    async def get_user(self, email):
        async with self.db.get_session() as session:
            result = await session.execute(
                text("SELECT * FROM users WHERE email = :email"),
                {"email": email}
            )
            return result.fetchone()

    # async def get_status_game_kode(kode):
    #     session = await get_session()
    #
    #
    #     session.close()

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

    async def get_info_game_invite_kode(self, code):
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

    async def insert_team(self, game_id, user_id, nickname):
        session = await self.db.get_session()
        await session.execute(
            text("INSERT INTO game_participants (game_id, user_id, nickname, score) "
                 "VALUES(:game_id, :user_id, :nickname, 0)"),
            {
                "game_id": game_id,
                "user_id": user_id,
                "nickname": nickname,
            }
        )
        await session.commit()
        await session.close()

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

    async def update_game_any_param(self, game_id, column, value):
        ALLOWED_COLUMNS = {"admin_user_id", "title", "join_code", "scheduled_at"}
        if column not in ALLOWED_COLUMNS:
            return "Такого столбца нет в таблице games"
        session = await self.db.get_session()
        await session.execute(
            text("UPDATE games "
                 f"SET {column} = :value "
                 "WHERE game_id = :game_id"),
            {
                "value": value,
                "game_id": game_id
            }
        )
        await session.commit()
        await session.close()
