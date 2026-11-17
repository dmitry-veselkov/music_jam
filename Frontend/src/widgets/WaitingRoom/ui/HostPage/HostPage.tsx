import { GameInfoCard } from "@/widgets/WaitingRoom/ui/GameInfoCard/GameInfoCard.tsx";
import { TeamList } from "@/widgets/WaitingRoom/ui/TeamList/TeamList.tsx";
import { GameCover } from "@/widgets/WaitingRoom/ui/GameCover/GameCover.tsx";
import styles from "./HostPage.module.css";
import { Button } from "@/shared/ui/Button/Button.tsx";
import { useRoomTeams } from "@/features/join-game/api/useRoomTeams.ts";
import { useJoinRoom } from "@/features/join-game/api/useJoinGame.ts";
import { useParams } from "react-router-dom";
import Header from "@/widgets/Header/Header.tsx";


const HostPage = () => {
  const { gameId } = useParams<{ gameId: string }>();
  const { teams } = useRoomTeams();
  useJoinRoom(gameId!, 'host', null);

  return (
    <>
      <Header showAuthButtons={false}/>
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
    </>
  );
};

export default HostPage;