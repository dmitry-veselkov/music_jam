import { useMutation } from "@tanstack/react-query";
import { questionApi } from "@/features/question-form/api/question.api.ts";
import { queryClient } from "@/shared/api/queryClient.ts";
import { questionCache } from "@/entities/question";

export const useUpdateQuestion = () => {
  return useMutation({
    mutationFn : questionApi.update,
    onSuccess: (updated) => questionCache.replace(queryClient, updated),
  })
}