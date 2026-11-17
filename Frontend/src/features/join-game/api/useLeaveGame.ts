import { useCallback, useState } from "react";
import { useSignalR } from "@/features/signalr-connection";

export const useLeaveGame = (gameId: string, teamId?: string | null) => {
  const { connection, state } = useSignalR();
  const [isLeaving, setIsLeaving] = useState(false);

  const leaveGame = useCallback(async (): Promise<boolean> => {
    if (state !== "connected" || !connection) return false;

    setIsLeaving(true);
    try {
      await connection.invoke("LeaveGame", gameId, teamId ?? null);
      return true;
    } catch (error) {
      console.error(error);
      return false;
    } finally {
      setIsLeaving(false);
    }
  }, [connection, state, gameId, teamId]);

  return { leaveGame, isLeaving };
};