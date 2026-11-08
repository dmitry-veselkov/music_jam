import type { Team } from "../model/types.ts";
import styles from "./TeamCard.module.css";

interface Props {
  team: Team;
  isCurrentUser?: boolean;
}

export const TeamCard = ({ team, isCurrentUser }: Props) => {
  return (
    <div className={styles.item}>
      <div className={styles.teamIcon}>
        <span>♟</span>
      </div>

      <span className={styles.name}>
        {team.teamName}
      </span>

      {isCurrentUser && (
        <span className={styles.currentUser}>
          Вы
        </span>
      )}

    </div>
  );
};