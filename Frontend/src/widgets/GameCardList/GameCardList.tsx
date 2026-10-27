import { useEffect, useState } from "react";
import { type GameDto, GameState, useGames } from "@/entities/game";
import styles from "@/widgets/PackCardList/PackCardList.module.css";
import { GameCard } from "@/entities/game";
import { DeleteGameButton, GameEditor } from "@/features/game";
import { Button } from "@/shared/ui/Button/Button.tsx";
import GameSection from "@/widgets/GameSection/GameSection.tsx";
import Modal from "@/shared/ui/Modal/Modal.tsx";

const PAGE_SIZE = 12;

const UPCOMING: GameState[] = [GameState.Scheduled, GameState.Pending];

const renderMainAction = (game: GameDto) =>
  game.state === GameState.Finished ? (
    <Button fullWidth shape="rounded" variant="secondary" onClick={() => {/* TODO: лог */}}>
      Посмотреть лог
    </Button>
  ) : (
    <Button fullWidth shape="rounded" onClick={() => {/* TODO: запуск игры */}}>
      Запустить
    </Button>
  );

const GameCardList = () => {
  const [page, setPage] = useState(0);
  const [editing, setEditing] = useState<GameDto | null>(null);
  const { data } = useGames({ skip: page * PAGE_SIZE, take: PAGE_SIZE });

  useEffect(() => {
    if (data && data.length === 0 && page > 0) setPage((p) => p - 1);
  }, [data, page]);

  const games = data ?? [];
  const upcoming = games.filter((g) => UPCOMING.includes(g.state));
  const past = games.filter((g) => g.state === GameState.Finished);

  const hasPrev = page > 0;
  const hasNext = games.length === PAGE_SIZE;

  const renderCard = (game: GameDto) => {
    const isFinished = game.state === GameState.Finished;
    return (
      <GameCard
        key={game.gameId}
        game={game}
        onClick={isFinished ? undefined : () => setEditing(game)}
        actions={isFinished ? undefined : <DeleteGameButton game={game} />}
        mainAction={renderMainAction(game)}
      />
    );
  };

  return (
    <div className={styles.sections}>
      <GameSection title="Ожидаемые" games={upcoming} renderCard={renderCard} />
      <GameSection title="Прошедшие" variant="past" games={past} renderCard={renderCard} />

      {(hasPrev || hasNext) && (
        <nav className={styles.pager} aria-label="Страницы">
          <Button size="sm" variant="secondary" disabled={!hasPrev} onClick={() => setPage((p) => p - 1)}>
            ← Назад
          </Button>
          <span className={styles.pageNumber}>{page + 1}</span>
          <Button size="sm" variant="secondary" disabled={!hasNext} onClick={() => setPage((p) => p + 1)}>
            Вперёд →
          </Button>
        </nav>
      )}
      <Modal isOpen={editing !== null} onClose={() => setEditing(null)} title="Редактирование игры">
        {editing && <GameEditor gameId={editing.gameId} onSaved={() => setEditing(null)} />}
      </Modal>
    </div>
  );
};

export default GameCardList;