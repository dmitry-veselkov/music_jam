import styles from "./ViewerPage.module.css";
import { GameInfoCard } from "@/widgets/WaitingRoom/ui/GameInfoCard/GameInfoCard.tsx";
import { TeamList } from "@/widgets/WaitingRoom/ui/TeamList/TeamList.tsx";
import { GameCover } from "@/widgets/WaitingRoom/ui/GameCover/GameCover.tsx";
import { useRoomTeams } from "@/features/join-game/api/useRoomTeams.ts";
import { useJoinRoom } from "@/features/join-game/api/useJoinGame.ts";
import { useParams } from "react-router-dom";

const ViewerPage = () => {
  const { gameId } = useParams<{ gameId: string }>();
  const { teams } = useRoomTeams();
  useJoinRoom(gameId!, 'viewer', null);

  if (!teams) {
    return (
      <div>
        Команд пока нет
      </div>
    )
  }

  // Пока пусть будет, в будущем у этого компонента будет 2 варианта работы, или с возможностью редактирования для админа, или просто как информация для ознакмоления для зрителей и игроков

  return (
    <main className={styles.container}>
      <h1>Зал ожидания</h1>

      <div className={styles.content}>
        <GameInfoCard />

        <TeamList teams={teams} />

        <GameCover />
      </div>
    </main>
  );
};

export default ViewerPage;