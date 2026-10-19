import { useMutation } from "@tanstack/react-query";
import { questionApi } from "@/features/question-form/api/question.api.ts";
import { questionCache } from "@/entities/question";
import { queryClient } from "@/shared/api/queryClient.ts";

export const useDeleteQuestion = () => {
  return useMutation({
    mutationFn: questionApi.delete,
    onSuccess: () => questionCache.invalidateAll(queryClient),
  })
}