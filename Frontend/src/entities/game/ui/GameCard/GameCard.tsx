import clsx from "clsx";
import { GameState, MODES, type GameDto } from "@/entities/game";
import { formatDate, formatTime } from "@/shared/lib/formatDate.ts";
import { type ReactNode } from "react";
import styles from "./GameCard.module.css";

type GameCardProps = {
  game: GameDto;
  onClick?: () => void;
  actions?: ReactNode;
  mainAction?: ReactNode;
};

export const GameCard = ({ game, onClick, actions, mainAction }: GameCardProps) => {
  const isPast = game.state === GameState.Finished;

  return (
    <li className={clsx(styles.card, onClick && styles.clickable)} onClick={onClick}>
      {actions && (
        <div className={styles.corner} onClick={(e) => e.stopPropagation()}>
          {actions}
        </div>
      )}
      <h3 className={styles.name}>{game.name}</h3>

      {isPast ? (
        <dl className={styles.facts}>
          <div className={styles.fact}>
            <dt>Проведена:</dt>
            <dd>{formatDate(game.date)}</dd>
          </div>
          <div className={styles.fact}>
            <dt>Режим:</dt>
            <dd>{MODES[game.mode].label}</dd>
          </div>
        </dl>
      ) : (
        <>
          <time className={styles.when} dateTime={game.date}>
            <span className={styles.time}>{formatTime(game.date)}</span>
            <span className={styles.date}>{formatDate(game.date)}</span>
          </time>
          <p className={styles.pack}>
            <span className={styles.packLabel}>Пак:</span>
            <span className={styles.packName}>{game.pack.name}</span>
          </p>
        </>
      )}

      {mainAction && (
        <div className={styles.action} onClick={(e) => e.stopPropagation()}>
          {mainAction}
        </div>
      )}
    </li>
  );
};