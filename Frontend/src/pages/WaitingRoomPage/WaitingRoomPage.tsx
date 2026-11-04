import { WaitingRoom } from "@/widgets/WaitingRoom/ui/WaitingRoom.tsx";
import { SignalRProvider } from "@/features/signalr-connection";
import { useParams } from "react-router-dom";

const WaitingRoomPage = () => {
  const { gameId } = useParams<{ gameId: string }>();
  if (!gameId) return null;

  return (
    <SignalRProvider>
      <WaitingRoom gameId={gameId} />
    </SignalRProvider>
  );
};

export default WaitingRoomPage;