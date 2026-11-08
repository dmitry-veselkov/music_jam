import styles from "./GameInfoCard.module.css";

// interface Props {
//   game: GameInfo;
// }

export const GameInfoCard = () => {
  return (
    <section className={styles.card}>
      <h2 className={styles.packName}>
        Название пака
      </h2>

      <div className={styles.schedule}>
        <div className={styles.scheduleItem}>
          <span>Дата</span>
          <div className={styles.value}>
            Дата
          </div>
        </div>

        <div className={styles.scheduleItem}>
          <span>Время</span>
          <div className={styles.value}>
            16.11.2026
          </div>
        </div>
      </div>

      <div className={styles.description}>
        <h3>Описание игры</h3>
        <p>Бебебе</p>
      </div>

      <div className={styles.author}>
        <img
          src="/IconAccount.svg"
          alt="IconAccount"
        />

        <span>Имя автора</span>
      </div>

      <div className={styles.mode}>
        <span>Режим игры:</span>
        <span className={styles.modeValue}>
          С ведущим
        </span>
      </div>
    </section>
  );
};