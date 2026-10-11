import { useMutation } from "@tanstack/react-query";
import { questionApi } from "@/features/question/api/question.api.ts";
import { questionCache } from "@/entities/question/api/question.cache.ts";
import { queryClient } from "@/shared/api/queryClient.ts";

export const useDeleteQuestion = () => {
  return useMutation({
    mutationFn: questionApi.delete,
    onSuccess: () => questionCache.invalidateAll(queryClient),
  })
}