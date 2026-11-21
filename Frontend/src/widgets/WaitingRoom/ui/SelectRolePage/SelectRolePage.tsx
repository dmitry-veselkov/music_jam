import { EnterTeamNameForm } from "@/features/join-game/ui/EnterTeamNameForm/EnterTeamNameForm.tsx";
import PlayerPage from "@/widgets/WaitingRoom/ui/PlayerPage/PlayerPage.tsx";
import ViewerPage from "@/widgets/WaitingRoom/ui/ViewerPage/ViewerPage.tsx";
import { useEffect, useState } from "react";
import type { Role } from "@/features/auth";
import { useCheckRole } from "@/features/auth/api/useCheckRole.ts";
import { useParams } from "react-router-dom";

type JoinData = {
  role: Omit<Role, 'host'>;
  teamName: string | null;
};

export const SelectRolePage = () => {
  const { gameId } = useParams<{gameId : string}>();
  const [joinData, setJoinData] = useState<JoinData | null>(null);
  const { data : checkRoleResponse } = useCheckRole(gameId!)

  useEffect(() => {
    if (checkRoleResponse) {
      setJoinData({
        role: checkRoleResponse.role,
        teamName: checkRoleResponse.teamName,
      });
    }
  }, [checkRoleResponse]);

  if (!joinData) {
    return (
      <EnterTeamNameForm
        onSubmit={(role, teamName) => {
          setJoinData({ role, teamName });
        }}
      />
    );
  }

  if (joinData.role === "viewer") {
    return <ViewerPage />;
  }

  return <PlayerPage teamName={joinData.teamName!} />;
};