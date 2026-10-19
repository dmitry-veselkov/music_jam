import { useMutation } from "@tanstack/react-query";
import { gameApi } from "@/features/game-form/api/game.api.ts";
import { queryClient } from "@/shared/api/queryClient.ts";

export const useUpsertGame = (options?: { onSuccess?: () => void }) => {
  return useMutation({
    mutationFn: gameApi.upsert,
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: ['games'] }); // префикс покрывает ['games', params]
      queryClient.invalidateQueries({ queryKey: ['game', id] }); // подставь свой ключ детали
      options?.onSuccess?.();
    },
  })
}