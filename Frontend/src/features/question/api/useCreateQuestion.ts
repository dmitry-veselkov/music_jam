import { useMutation } from "@tanstack/react-query";
import { questionMutations } from "@/features/question";
import { questionCache } from "@/entities/question";
import { queryClient } from "@/shared/api/queryClient.ts";

export const useCreateQuestion = () => {
  return useMutation({
    mutationFn : questionMutations.create,
    onSuccess: () => questionCache.invalidateAll(queryClient),
  })
}