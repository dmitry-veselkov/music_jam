import { SignalRProvider } from "@/features/signalr-connection";
import { useParams } from "react-router-dom";
import { SelectRolePage } from "@/widgets/WaitingRoom/ui/SelectRolePage/SelectRolePage";
import HostPage from "@/widgets/WaitingRoom/ui/HostPage/HostPage.tsx";
import { useCheckHost } from "@/features/auth/api/useCheckHost.ts";

const WaitingRoomPage = () => {
  const { gameId } = useParams<{ gameId: string }>();
  const { data : isHost } = useCheckHost(gameId!);

  if (!gameId) return null;

  return (
    <SignalRProvider>
      {isHost && <HostPage />}
      {!isHost && <SelectRolePage/>}
    </SignalRProvider>
  );
};

export default WaitingRoomPage;