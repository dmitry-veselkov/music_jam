import type { GameDto } from "@/entities/game";
import styles from "./GameCard.module.css"
import { type ReactNode } from "react";

type GameCardProps = {
  game: GameDto;
  cornerActions?: ReactNode;
  mainAction?: ReactNode;
};

export const GameCard = ({ game, cornerActions, mainAction }: GameCardProps) => {
  return (
    <div className={styles.card}>
      {cornerActions && <div className={styles.cornerActions}>{cornerActions}</div>}
      <div className={styles.nameAndButton}>
        <div className={styles.name}>
          {game.name}
          {mainAction}
        </div>
      </div>
    </div>
  );
};