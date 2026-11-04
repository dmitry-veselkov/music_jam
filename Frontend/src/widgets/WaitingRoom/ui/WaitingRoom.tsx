import { useHubEvent, useSignalR } from "@/features/signalr-connection";
import { useState } from "react";
import type { Team } from "@/entities/team/model/types.ts";
import { useJoinRoom } from "@/features/join-game/model/useJoinGame.ts";
import { EnterTeamNameForm } from "@/features/join-game/ui/EnterTeamNameForm.tsx";
import { TeamCard } from "@/entities/team/ui/TeamCard.tsx";

export const WaitingRoom = ({ gameId } : { gameId : string }) => {
  const { state } = useSignalR();
  const [teams, setTeams] = useState<Team[]>([]);
  const [teamName, setTeamName] = useState<string | null>(null);

  const { currentTeam } = useJoinRoom(gameId, teamName);

  useHubEvent<Team[]>('RoomState', setTeams);
  useHubEvent<Team>('TeamJoined', (team) => setTeams((prev) => [...prev, team]));
  useHubEvent<string>('TeamLeft', (teamId) => setTeams((prev) => prev.filter((team) => team.teamId !== teamId)));

  if (state === 'connecting' || state === 'reconnecting') {
    return <div>Подключение...</div>;
  }

  if (!teamName) {
    return <EnterTeamNameForm onSubmit={setTeamName} />;
  }

  return (
    <div className="waiting-room">
      <h2>Команд в зале: {teams.length}</h2>
      <ul className="waiting-room__list">
        {teams.map((team) => (
          <TeamCard
            key={team.teamId}
            team={team}
            isCurrentUser={team.teamId === currentTeam?.teamId}
          />
        ))}
      </ul>
    </div>
  );
};