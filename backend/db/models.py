from sqlalchemy import BigInteger, String, UniqueConstraint, Text, ForeignKey, TIMESTAMP, Integer, CheckConstraint
from sqlalchemy.orm import Mapped, mapped_column
from database import Base

class User(Base):
    __tablename__ = "users"

    id = mapped_column(BigInteger, primary_key=True)
    name = mapped_column(String(100), nullable=False)
    password_hash = mapped_column(String(255), nullable=False)
class Game(Base):
    __tablename__ = "games"

    id = mapped_column(BigInteger, primary_key=True)
    admin_user_id = mapped_column(
        BigInteger,
        ForeignKey("users.id", ondelete="RESTRICT"),
        nullable=False
    )
    title = mapped_column(String(200), nullable=False)
    join_code = mapped_column(String(100), nullable=False, unique=True)
    join_token = mapped_column(String(100), nullable=False, unique=True)
    scheduled_at = mapped_column(TIMESTAMP, nullable=True)
class GameParticipant(Base):
    __tablename__ = "game_participants"
    __table_args__ = (
        UniqueConstraint("game_id", "nickname"),
    )

    id = mapped_column(BigInteger, primary_key=True)
    game_id = mapped_column(
        BigInteger,
        ForeignKey("games.id", ondelete="CASCADE"),
        nullable=False
    )
    user_id = mapped_column(
        BigInteger,
        ForeignKey("users.id", ondelete="SET NULL"),
        nullable=True
    )
    nickname = mapped_column(String(100), nullable=False)
    score = mapped_column(Integer, nullable=False, default=0)
class Song(Base):
    __tablename__ = "songs"
    __table_args__ = (
        CheckConstraint("price > 0"),
    )

    id = mapped_column(BigInteger, primary_key=True)
    title = mapped_column(String(200), nullable=False)
    artist = mapped_column(String(200), nullable=False)
    category = mapped_column(String(100), nullable=False)
    price = mapped_column(Integer, nullable=False)
    file_url = mapped_column(Text, nullable=False)
class GameSong(Base):
    __tablename__ = "game_songs"
    __table_args__ = (
        UniqueConstraint("game_id", "song_id"),
    )

    id = mapped_column(BigInteger, primary_key=True)
    game_id = mapped_column(
        BigInteger,
        ForeignKey("games.id", ondelete="CASCADE"),
        nullable=False
    )
    song_id = mapped_column(
        BigInteger,
        ForeignKey("songs.id", ondelete="RESTRICT"),
        nullable=False
    )