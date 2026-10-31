import { useMutation } from "@tanstack/react-query";
import { questionMutations } from "@/features/question";
import { questionCache } from "@/entities/question";
import { queryClient } from "@/shared/api/queryClient.ts";

export const useDeleteQuestion = () => {
  return useMutation({
    mutationFn: questionMutations.delete,
    onSuccess: () => questionCache.invalidateAll(queryClient),
  })
}