from database import get_session
from sqlalchemy import text
import asyncio

async def get_user_name_pass(name, hash_password):
    session = await get_session()
    result = await session.execute(
        text("SELECT * FROM users WHERE name = :name AND password_hash = :password" ),
        {
            "password" : hash_password,
            "name" : name
        }
    )
    row = result.fetchone()
    await session.close()
    if not row:
        return None
    return row

# async def get_status_game_kode(kode):
#     session = await get_session()
#
#
#     session.close()

async def insert_user(name, hash_password):
    session = await get_session()
    await session.execute(
        text("INSERT INTO users (name, password_hash) VALUES(:name, :hash_password)"),
        {
            "name" : name,
            "hash_password" : hash_password
        }
    )
    await session.commit()
    await session.close()

async def get_info_game_invite_kode(code):
    session = await get_session()
    result = await session.execute(
        text("SELECT * FROM games WHERE join_code = :join_code" ),
        {
            "join_code" : code
        }
    )
    row = result.fetchall()
    await session.close()
    if not row:
        return None
    return row

async def create_game(admin_id, title, join_code, scheduled_at):
    session = await get_session()
    await session.execute(
        text("INSERT INTO games (admin_user_id, title, join_code, scheduled_at) "
             "VALUES(:admin_id, :title, :join_code, :scheduled_at)"),
        {
            "admin_id" : admin_id,
            "title" : title,
            "join_code" : join_code,
            "scheduled_at" : scheduled_at
        }
    )
    await session.commit()
    await session.close()

async def insert_team(game_id, user_id, nickname):
    session = await get_session()
    await session.execute(
        text("INSERT INTO game_participants (game_id, user_id, nickname, score) "
             "VALUES(:game_id, :user_id, :nickname, 0)"),
        {
            "game_id" : game_id,
            "user_id" : user_id,
            "nickname" : nickname,
        }
    )
    await session.commit()
    await session.close()

async def update_score_team(game_id, nickname, add_score):
    # TODO: пока хз по нику или по айди
    session = await get_session()
    await session.execute(
        text("UPDATE game_participants"
             "SET score = score + :add_score"
             "WHERE game_id = :game_id AND nickname = :nickname"),
        {
            "game_id" : game_id,
            "nickname" : nickname,
            "add_score" : add_score
        }
    )
    await session.commit()
    await session.close()

async def update_game_any_param(game_id, column, value):
    ALLOWED_COLUMNS = {"admin_user_id", "title", "join_code", "scheduled_at"}
    if column not in ALLOWED_COLUMNS:
        return "Такого столбца нет в таблице games"
    session = await get_session()
    await session.execute(
        text("UPDATE games"
             f"SET {column} = :value"
             "WHERE game_id = :game_id"),
        {
            "value" : value,
            "game_id" : game_id
        }
    )
    await session.commit()
    await session.close()

# async def main():
#     await insert_user("tim", "65")
#     res = await get_user_name_pass("tim", "65")
#     print(res)
# if __name__ == "__main__":
#     asyncio.run(main())