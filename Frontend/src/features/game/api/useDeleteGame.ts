import { useMutation } from "@tanstack/react-query";
import { gameMutations } from "@/features/game";
import { queryClient } from "@/shared/api/queryClient.ts";

export const useDeleteGame = () => {
  return useMutation({
    mutationFn: (id: string) => gameMutations.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({queryKey: ['games']})
    }
  })
}