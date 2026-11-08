import { PackCard, usePackCardList } from "@/entities/pack";
import styles from "./PackCardList.module.css";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { DeletePackButton } from "@/features/pack/ui/DeletePackButton/DeletePackButton.tsx";
import { Button } from "@/shared/ui/Button/Button.tsx";
import { useStartGame } from "@/features/start-game/model/useStartGame.ts";

const PAGE_SIZE = 12;

const PackCardList = () => {
  const [page, setPage] = useState(0);
  const navigate = useNavigate();
  const { data } = usePackCardList({ skip: page * PAGE_SIZE, take: PAGE_SIZE });
  const create = useStartGame();

  useEffect(() => {
    if (data && data.length === 0 && page > 0) setPage((p) => p - 1);
  }, [data, page]);

  const handleStartGame = async ({ packId } : { packId : string }) => {
    const { gameId } = await create.mutateAsync({ packId });
    navigate(`/games/${gameId}/waiting-room`);
  }

  const items = data ?? []

  return (
    <div>
      <ul className={styles.packList}>
        {items.map((pack) => (
          <PackCard
            key={pack.packId}
            pack={pack}
            onClick={() => navigate(`/packs/${pack.packId}`)}
            deleteButton={<DeletePackButton pack={pack} />}
            startButton={<Button text={"Запустить"} onClick={() => handleStartGame({ packId : pack.packId})}/>}
          />
        ))}
      </ul>
    </div>
  );
};

export default PackCardList;