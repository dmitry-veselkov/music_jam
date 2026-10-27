import { useState } from "react";
import { ConfirmDialog } from "@/shared/ui/ConfirmDialog/ConfirmDialog";
import type { GameDto } from "@/entities/game";
import { useDeleteGame } from "@/features/game";

export const DeleteGameButton = ({ game }: { game: GameDto }) => {
  const [open, setOpen] = useState(false);
  const deleteGame = useDeleteGame();

  return (
    <>
      <img
        src="/Garbage.svg"
        alt="GarbageGame"
        onClick={() => setOpen(true)}
        aria-label="Удалить игру"
      />
      <ConfirmDialog
        open={open}
        title="Удалить игру?"
        description={`Игра «${game.name}» будет удалена без возможности восстановления.`}
        isLoading={deleteGame.isPending}
        onConfirm={() =>
          deleteGame.mutate(game.gameId, { onSuccess: () => setOpen(false) })
        }
        onCancel={() => setOpen(false)}
      />
    </>
  );
};