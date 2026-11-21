import styles from "./JoinGamePanel.module.css"
import JoinByCodeForm from "@/features/join-game/ui/JoinByCodeForm/JoinByCodeForm.tsx";

const JoinGamePanel = () => {
  return (
    <section className={styles.joinGamePanel}>
      <div className={styles.joinGamePanelHeading}>
        <h2>Присоединиться к игре</h2>
        <p>Введите шестизначный код комнаты</p>
      </div>

      <JoinByCodeForm  />
    </section>
  );
};

export default JoinGamePanel;