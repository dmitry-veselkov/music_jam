import type { PackCardDto } from "@/entities/pack";
import styles from "./PackCard.module.css";
import { useNavigate } from "react-router";

export const PackCard = (info : PackCardDto) => {
  const navigate = useNavigate();

  return (
    <div
      className={styles.packCard}
      onClick={() => navigate(`/packs/${info.packId}`)}
    >
      <h2 className={styles.name}>{info.name}</h2>
      <div className={styles.dates}>
        <time className={styles.date}>
          <span>Обновлён:</span>
          {new Date(info.lastUpdateDate).toLocaleDateString("ru-RU")}
        </time>
        <time className={styles.date}>
          <span>Создан:</span>
          {new Date(info.creationDate).toLocaleDateString("ru-RU")}
        </time>
      </div>
    </div>
  );
};
