import { useMutation } from "@tanstack/react-query";
import { gameApi } from "@/features/game-form/api/game.api.ts";

export const useDeleteGame = () => {
  return useMutation({
    mutationFn: gameApi.delete,
  })
}