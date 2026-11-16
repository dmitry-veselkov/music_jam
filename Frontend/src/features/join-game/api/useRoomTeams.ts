import { useHubEvent } from "@/features/signalr-connection";
import type { Team } from "@/entities/team";
import { useState } from "react";

export const useRoomTeams = () => {
  const [teams, setTeams] = useState<Team[]>([]);

  useHubEvent<Team[]>("RoomState", setTeams);

  useHubEvent<Team>("TeamJoined", (team) => {
    setTeams((prev) => {
      if (prev.some((t) => t.teamId === team.teamId)) {
        return prev;
      }

      return [...prev, team];
    });
  });

  useHubEvent<Team["teamId"]>("TeamLeft", (teamId) => {
    setTeams((prev) =>
      prev.filter((team) => team.teamId !== teamId)
    );
  });

  return { teams };
};