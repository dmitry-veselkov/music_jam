import Header from "@/widgets/Header/Header.tsx";
import styles from "./GamesPage.module.css"
import { Button } from "@/shared/ui/Button/Button.tsx";
import GameCardList from "@/widgets/GameCardList/GameCardList.tsx";
import { useState } from "react";
import Modal from "@/shared/ui/Modal/Modal.tsx";
import { GameEditor } from "@/features/game";

const GamesPage = () => {
  const [isCreateOpen, setIsCreateOpen] = useState(false);

  return (
    <div>
      <Header showAuthButtons={false}/>
      <div className={styles.gamesContent}>
        <div className={styles.gameOptions}>
          <h1>Мои игры</h1>
          <Button
            text="Создать игру"
            onClick={() => setIsCreateOpen(true)}
          />
        </div>
        <GameCardList />
      </div>
      <Modal
        isOpen={isCreateOpen}
        onClose={() => setIsCreateOpen(false)}
        title="Создание игры"
      >
        {isCreateOpen && (
          <GameEditor onSaved={() => setIsCreateOpen(false)} />
        )}
      </Modal>
    </div>
  );
};

export default GamesPage;