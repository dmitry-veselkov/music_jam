import { useEffect, useState } from "react";
import { useGames } from "@/entities/game";
import styles from "@/widgets/PackCardList/PackCardList.module.css";
import { GameCard } from "@/entities/game";
import { DeleteGameButton } from "@/features/game";
import { EditGameButton } from "@/features/game";

const PAGE_SIZE = 12;
// Для будущего внедрения проведенных игр
const GameCardList = () => {
  const [page, setPage] = useState(0);
  const { data } = useGames({ skip: page * PAGE_SIZE, take: PAGE_SIZE });

  useEffect(() => {
    if (data && data.length === 0 && page > 0) setPage((p) => p - 1);
  }, [data, page]);

  const items = data ?? []

  return (
    <div>
      <ul className={styles.packList}>
        {items.map((game) => (
          <GameCard
            key={game.gameId}
            game={game}
            cornerActions={<DeleteGameButton game={game} />}
            mainAction={<EditGameButton gameId={game.gameId} />}
          />
        ))}
      </ul>
    </div>
  );
};

export default GameCardList;