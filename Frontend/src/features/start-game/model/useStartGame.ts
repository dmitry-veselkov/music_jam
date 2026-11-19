import { useMutation } from "@tanstack/react-query";
import { gameProcess } from "@/features/start-game/api/game-process.ts";

export const useStartGame = () => {
  return useMutation({
    mutationFn: gameProcess.create,
  });
}