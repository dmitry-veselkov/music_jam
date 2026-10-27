import type { PackCardDto } from "@/entities/pack";
import styles from "./PackCard.module.css";
import { type ReactNode} from "react";
import { formatDate, formatTime } from "@/shared/lib/formatDate.ts";

type PackCardProps = {
  pack: PackCardDto;
  onClick?: () => void;
  actions?: ReactNode;
}

export const PackCard = ({ pack, onClick, actions }: PackCardProps) => {
  return (
    <li className={styles.packCard} onClick={onClick}>
      {actions && (
        <div className={styles.actions} onClick={(e) => e.stopPropagation()}>
          {actions}
        </div>
      )}
      <h2 className={styles.name}>{pack.name}</h2>

      <div className={styles.info}>
        <div className={styles.column}>
          <section className={styles.block}>
            <h3 className={styles.blockTitle}>Дата создания</h3>
            <time className={styles.value} dateTime={pack.creationDate}>
              {formatDate(pack.creationDate)}
            </time>
          </section>
          <section className={styles.block}>
            <h3 className={styles.blockTitle}>Последнее изменение</h3>
            <time className={styles.value} dateTime={pack.lastUpdateDate}>
              <span className={styles.time}>{formatTime(pack.lastUpdateDate)}</span>{" "}
              {formatDate(pack.lastUpdateDate)}
            </time>
          </section>
        </div>

        <div className={`${styles.column} ${styles.usedColumn}`}>
          <h3 className={styles.blockTitle}>Используется</h3>
          {pack.usages?.length ? (
            <ul className={styles.games}>
              {pack.usages.map((game) => (
                <li key={game.gameId} className={styles.game}>
                  {/*TODO: сделать usages*/}
                  {game.name}
                </li>
              ))}
            </ul>
          ) : (
            <p className={styles.empty}>Пока нигде</p>
          )}
        </div>
      </div>
    </li>
  );
};
