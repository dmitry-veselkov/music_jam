import type { Team } from "@/entities/team/model/types";
import { useEffect, useState } from "react";
import { useSignalR } from "@/features/signalr-connection";
import type { Role } from "@/features/auth";


export const useJoinRoom = (
  gameId: string,
  teamName: string | null,
  role : Role
) => {
  const { connection, state } = useSignalR();
  const [currentTeam, setCurrentTeam] = useState<Team | null>(null);

  useEffect(() => {
    if (state !== 'connected' || !connection || !teamName) return;
    if (role === "player" && !teamName) return;

    connection
      .invoke<Team>('JoinGame', gameId, teamName, role)
      .then((team) => setCurrentTeam(team))
      .catch(console.error);

    return () => {
      if (currentTeam) {
        connection.invoke('LeaveGame', gameId, currentTeam.teamId).catch(() => {});
      }
    };
  }, [connection, state, gameId, teamName, role]);

  return { currentTeam };
};