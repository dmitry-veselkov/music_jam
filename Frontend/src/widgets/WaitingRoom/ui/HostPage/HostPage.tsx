import { GameInfoCard } from "@/widgets/WaitingRoom/ui/GameInfoCard/GameInfoCard.tsx";
import { TeamList } from "@/widgets/WaitingRoom/ui/TeamList/TeamList.tsx";
import { GameCover } from "@/widgets/WaitingRoom/ui/GameCover/GameCover.tsx";
import styles from "./HostPage.module.css";
import { Button } from "@/shared/ui/Button/Button.tsx";
import { useHubEvent } from "@/features/signalr-connection";
import type { Team } from "@/entities/team";
import { useState } from "react";


const HostPage = () => {
  // В будущем, для настроек игры
  // const { id } = useParams<{id : string}>();
  const [teams, setTeams] = useState<Team[]>([]);

  useHubEvent<Team[]>('RoomState', setTeams);
  useHubEvent<Team>('TeamJoined', (team) => setTeams((prev) => [...prev, team]));
  useHubEvent<string>('TeamLeft', (teamId) => setTeams((prev) => prev.filter((team) => team.teamId !== teamId)));

  return (
    <main className={styles.container}>
      <h1>Зал ожидания</h1>

      <div className={styles.content}>
        <GameInfoCard />

        <TeamList teams={teams} />

        <GameCover />
      </div>

      <Button
        text={"Начать игру"}
      />
    </main>
  );
};

export default HostPage;