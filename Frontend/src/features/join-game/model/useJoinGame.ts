import type { Team } from "@/entities/team/model/types";
import { useEffect, useState } from "react";
import { useSignalR } from "@/features/signalr-connection";


export const useJoinRoom = (gameId: string, teamName: string | null) => {
  const { connection, state } = useSignalR();
  const [currentTeam, setCurrentTeam] = useState<Team | null>(null);

  useEffect(() => {
    if (state !== 'connected' || !connection || !teamName) return;

    connection
      .invoke<Team>('JoinGame', gameId, teamName)
      .then((team) => setCurrentTeam(team))
      .catch(console.error);

    return () => {
      if (currentTeam) {
        connection.invoke('LeaveGame', gameId, currentTeam.teamId).catch(() => {});
      }
    };
  }, [connection, state, gameId, teamName]);

  return { currentTeam };
};