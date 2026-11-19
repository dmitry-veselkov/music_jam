import styles from "./TeamList.module.css";
import { type Team, TeamCard } from "@/entities/team";

interface Props {
  teams: Team[];
  myTeamId? : string;
}

export const TeamList = ({ teams, myTeamId }: Props) => {
  return (
    <section className={styles.container}>
      <h2>Список команд</h2>

      <div className={styles.list}>
        {teams.map((team) => (
          <TeamCard
            key={team.teamId}
            team={team}
            isCurrentUser={team.teamId === myTeamId}
          />
        ))}
      </div>
    </section>
  );
};