import { useState } from "react";
import { Button } from "@/shared/ui/Button/Button";
import Modal from "@/shared/ui/Modal/Modal";
import { GameEditor } from "@/features/game";

export const EditGameButton = ({ gameId }: { gameId: string }) => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <Button text="Редактировать" onClick={() => setIsOpen(true)} />
      <Modal isOpen={isOpen} onClose={() => setIsOpen(false)} title="Редактирование игры">
        <GameEditor gameId={gameId} onSaved={() => setIsOpen(false)} />
      </Modal>
    </>
  );
};