import { EnterTeamNameForm } from "@/features/join-game/ui/EnterTeamNameForm.tsx";
import PlayerPage from "@/widgets/WaitingRoom/ui/PlayerPage/PlayerPage.tsx";
import ViewerPage from "@/widgets/WaitingRoom/ui/ViewerPage/ViewerPage.tsx";
import { useState } from "react";
import type { Role } from "@/features/auth";

type JoinData = {
  name: string;
  role: Omit<Role, 'host'>;
};

export const SelectRolePage = () => {
  const [joinData, setJoinData] = useState<JoinData | null>(null);

  if (!joinData) {
    return (
      <EnterTeamNameForm
        onSubmit={(name, role) => {
          setJoinData({ name, role });
        }}
      />
    );
  }

  if (joinData.role === "viewer") {
    return <ViewerPage />;
  }

  return <PlayerPage teamName={joinData.name} />;
};