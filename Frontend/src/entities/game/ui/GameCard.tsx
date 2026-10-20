import type { GameDto } from "@/entities/game/model/types.ts";
import { Button } from "@/shared/ui/Button/Button.tsx";
import styles from "./GameCard.module.css"
import { useState } from "react";
import Modal from "@/shared/ui/Modal/Modal.tsx";
import GameEditor from "@/features/game-form/ui/GameEditor.tsx";
import { useDeleteGame } from "@/features/game-form/api/useDeleteGame.ts";
import { ConfirmDialog } from "@/shared/ui/ConfirmDialog/ConfirmDialog.tsx";

const GameCard = ({ game } : { game : GameDto}) => {
  const [isOpen, setIsOpen] = useState(false);
  const deleteGame = useDeleteGame();
  const [gameToDelete, setGameToDelete] = useState<{ gameId: string; name: string } | null>(null);
  
  const handleConfirm = () => {
    if (!gameToDelete) return;
    deleteGame.mutate(gameToDelete.gameId, {
      onSuccess: () => setGameToDelete(null),
    });
  };
  
  return (
    <div className={styles.card}>
      <div
        className={styles.deleteIcon}
        onClick={(e) => {
          e.stopPropagation();
          setGameToDelete(game)
        }}
      >
        🗑️
      </div>
      <div className={styles.nameAndButton}>
        <div className={styles.name}>
          {game.name}
        </div>
        <Button
          text={"Запустить"}
          onClick={() => setIsOpen(true)}
        />
      </div>
      <Modal isOpen={isOpen} onClose={() => {setIsOpen(false)}} title="Редактирование игры">
        <GameEditor gameId={game.gameId} onSaved={() => setIsOpen(false)} />
      </Modal>
      <ConfirmDialog
        open={gameToDelete !== null}
        title="Удалить игру?"
        description={`Игра «${gameToDelete?.name ?? ""}» будет удалена без возможности восстановления.`}
        isLoading={deleteGame.isPending}
        onConfirm={handleConfirm}
        onCancel={() => setGameToDelete(null)}
      />
    </div>
  );
};

export default GameCard;