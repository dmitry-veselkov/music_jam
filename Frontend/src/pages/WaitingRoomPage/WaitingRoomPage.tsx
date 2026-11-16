import { SignalRProvider } from "@/features/signalr-connection";
import { useParams } from "react-router-dom";
import { useCheckRole } from "@/features/auth/api/useCheckRole.ts";
import { SelectRolePage } from "@/widgets/WaitingRoom/ui/SelectRolePage/SelectRolePage";
import HostPage from "@/widgets/WaitingRoom/ui/HostPage/HostPage.tsx";

const WaitingRoomPage = () => {
  const { gameId } = useParams<{ gameId: string }>();
  const { data } = useCheckRole(gameId!);
  console.log(data);
  if (!gameId) return null;

  return (
    <SignalRProvider>
      {data?.role === 'host' && <HostPage />}
      {data?.role !== 'host' && <SelectRolePage/>}
    </SignalRProvider>
  );
};

export default WaitingRoomPage;