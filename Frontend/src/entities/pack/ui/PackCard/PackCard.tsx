import type { PackCardDto } from "@/entities/pack";
import styles from "./PackCard.module.css";
import { type ReactNode} from "react";

type PackCardProps = {
  pack: PackCardDto;
  onClick?: () => void;
  actions?: ReactNode;
}

export const PackCard = ({ pack, onClick, actions }: PackCardProps) => {
  return (
    <div className={styles.packCard} onClick={onClick}>
      {actions && (
        <div className={styles.actions} onClick={(e) => e.stopPropagation()}>
          {actions}
        </div>
      )}
      <h2 className={styles.name}>{pack.name}</h2>
      <div className={styles.dates}>
        <time className={styles.date}>
          <span>Обновлён:</span>
          {new Date(pack.lastUpdateDate).toLocaleDateString("ru-RU")}
        </time>
        <time className={styles.date}>
          <span>Создан:</span>
          {new Date(pack.creationDate).toLocaleDateString("ru-RU")}
        </time>
      </div>
    </div>
  );
};
