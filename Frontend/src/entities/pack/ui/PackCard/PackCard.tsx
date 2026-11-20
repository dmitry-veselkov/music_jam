import type { PackCardDto } from "@/entities/pack";
import styles from "./PackCard.module.css";
import { type ReactNode} from "react";

type PackCardProps = {
  pack: PackCardDto;
  onClick?: () => void;
  deleteButton?: ReactNode;
  startButton?: ReactNode;
}

export const PackCard = ({ pack, onClick, deleteButton, startButton }: PackCardProps) => {
  return (
    <div className={styles.packCard} onClick={onClick}>
      {deleteButton && (
        <div className={styles.actions} onClick={(e) => e.stopPropagation()}>
          {deleteButton}
        </div>
      )}
      <h2 className={styles.name}>{pack.name}</h2>
      <div className={styles.dates}>
        <time className={styles.date}>
          <span>Обновлён:</span>
          {new Date(pack.updatedAtUtc).toLocaleDateString("ru-RU")}
        </time>
        <time className={styles.date}>
          <span>Создан:</span>
          {new Date(pack.createdAtUtc).toLocaleDateString("ru-RU")}
        </time>
      </div>
      {startButton && (
        <div className={styles.actions} onClick={(e) => e.stopPropagation()}>
          {startButton}
        </div>
      )}
    </div>
  );
};
