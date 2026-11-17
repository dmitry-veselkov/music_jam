import styles from "./PlayerPage.module.css";
import { GameInfoCard } from "@/widgets/WaitingRoom/ui/GameInfoCard/GameInfoCard.tsx";
import { TeamList } from "@/widgets/WaitingRoom/ui/TeamList/TeamList.tsx";
import { GameCover } from "@/widgets/WaitingRoom/ui/GameCover/GameCover.tsx";
import { useJoinRoom } from "@/features/join-game/api/useJoinGame.ts";
import { useParams } from "react-router-dom";
import { useRoomTeams } from "@/features/join-game/api/useRoomTeams.ts";

type Props = {
  teamName : string;
}

const PlayerPage = ({ teamName } : Props) => {
  const { gameId } = useParams<{ gameId: string }>();
  const { teams } = useRoomTeams();
  const { currentTeam } = useJoinRoom(gameId!, 'player', teamName);

  // Пока пусть будет, в будущем у этого компонента будет 2 варианта работы, или с возможностью редактирования для админа, или просто как информация для ознакмоления для зрителей и игроков

  return (
    <main className={styles.container}>
      <h1>Зал ожидания</h1>

      <div className={styles.content}>
        <GameInfoCard />

        <TeamList
          teams={teams}
          myTeamId={currentTeam?.teamId}
        />

        <GameCover />
      </div>
    </main>
  );
};

export default PlayerPage;