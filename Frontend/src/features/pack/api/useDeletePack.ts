import { useMutation } from "@tanstack/react-query";
import { queryClient } from "@/shared/api/queryClient.ts";
import { packMutations } from "@/features/pack/api/pack.mutations.ts";
import { packKeys } from "@/entities/pack/api/pack.queries.ts";

export const useDeletePack = () => {
  return useMutation({
    mutationFn: (id: string) => packMutations.delete(id),
    onSuccess: (_, id) => {
      queryClient.removeQueries({ queryKey: packKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: packKeys.lists() });
    },
  });
}