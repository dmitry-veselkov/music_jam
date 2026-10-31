import { useMutation } from "@tanstack/react-query";
import { gameMutations } from "@/features/game";
import { queryClient } from "@/shared/api/queryClient.ts";

export const useUpsertGame = (options?: { onSuccess?: () => void }) => {
  return useMutation({
    mutationFn: gameMutations.upsert,
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: ['games'] }); // префикс покрывает ['games', params]
      queryClient.invalidateQueries({ queryKey: ['game', id] }); // подставь свой ключ детали
      options?.onSuccess?.();
    },
  })
}