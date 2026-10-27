import Header from "@/widgets/Header/Header.tsx";
import styles from "./GamesPage.module.css"
import { Button } from "@/shared/ui/Button/Button.tsx";
import GameCardList from "@/widgets/GameCardList/GameCardList.tsx";
import { useState } from "react";
import Modal from "@/shared/ui/Modal/Modal.tsx";
import { GameEditor } from "@/features/game";
import PageHeader from "@/shared/ui/PageHeader/PageHeader";
import Layout from "@/shared/ui/Layout/Layout.tsx";

const GamesPage = () => {
  const [isCreateOpen, setIsCreateOpen] = useState(false);

  return (
    <Layout>
      <Header showAuthButtons={false}/>
      <main className={styles.content}>
        <PageHeader title="Игры">
          <Button startIcon="+" onClick={() => setIsCreateOpen(true)}>
            Создать новую игру
          </Button>
        </PageHeader>
        <GameCardList />
      </main>
      <Modal
        isOpen={isCreateOpen}
        onClose={() => setIsCreateOpen(false)}
        title="Создание игры"
      >
        {isCreateOpen && (
          <GameEditor onSaved={() => setIsCreateOpen(false)} />
        )}
      </Modal>
    </Layout>
  );
};

export default GamesPage;