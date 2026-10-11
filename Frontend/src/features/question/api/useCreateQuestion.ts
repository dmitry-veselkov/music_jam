import { useMutation } from "@tanstack/react-query";
import { questionApi } from "@/features/question/api/question.api.ts";
import { questionCache } from "@/entities/question-form";
import { queryClient } from "@/shared/api/queryClient.ts";

export const useCreateQuestion = () => {
  return useMutation({
    mutationFn : questionApi.create,
    onSuccess: () => questionCache.invalidateAll(queryClient),
  })
}