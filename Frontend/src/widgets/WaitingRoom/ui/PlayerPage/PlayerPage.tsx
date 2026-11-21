import styles from "./PlayerPage.module.css";
import { GameInfoCard } from "@/widgets/WaitingRoom/ui/GameInfoCard/GameInfoCard.tsx";
import { TeamList } from "@/widgets/WaitingRoom/ui/TeamList/TeamList.tsx";
import { GameCover } from "@/widgets/WaitingRoom/ui/GameCover/GameCover.tsx";
import { useJoinRoom } from "@/features/join-game/api/useJoinGame.ts";
import { useParams } from "react-router-dom";
import { useRoomTeams } from "@/features/join-game/api/useRoomTeams.ts";
import Header from "@/widgets/Header/ui/Header.tsx";
import { useNavigate } from "react-router";
import { useLeaveGame } from "@/features/join-game/api/useLeaveGame.ts";

type Props = {
  teamName : string;
}

const PlayerPage = ({ teamName } : Props) => {
  const navigate = useNavigate();
  const { gameId } = useParams<{ gameId: string }>();
  const { teams } = useRoomTeams();
  const { currentTeam } = useJoinRoom(gameId!, 'player', teamName);
  const { leaveGame, isLeaving } = useLeaveGame(gameId!, currentTeam?.teamId);

  const handleLeave = async () => {
    const ok = await leaveGame();
    if (ok) navigate("/");
  }

  // Пока пусть будет, в будущем у этого компонента будет 2 варианта работы, или с возможностью редактирования для админа, или просто как информация для ознакмоления для зрителей и игроков

  return (
    <>
      <Header />
      <main className={styles.container}>
        <div className={styles.footer}>
          <h1>Зал ожидания</h1>
          <button
            type="button"
            className={styles.close}
            onClick={handleLeave}
            disabled={isLeaving}
          >
            Выйти из игры
          </button>
        </div>

        <div className={styles.content}>
          <GameInfoCard />

          <TeamList
            teams={teams}
            myTeamId={currentTeam?.teamId}
          />

          <GameCover />
        </div>
      </main>
    </>
  );
};

export default PlayerPage;