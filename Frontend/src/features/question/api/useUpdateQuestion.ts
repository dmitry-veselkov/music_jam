import { useMutation } from "@tanstack/react-query";
import { questionMutations } from "@/features/question";
import { queryClient } from "@/shared/api/queryClient.ts";
import { questionCache } from "@/entities/question";

export const useUpdateQuestion = () => {
  return useMutation({
    mutationFn : questionMutations.update,
    onSuccess: (updated) => questionCache.replace(queryClient, updated),
  })
}