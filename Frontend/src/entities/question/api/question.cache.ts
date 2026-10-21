import type { QueryClient } from "@tanstack/react-query";
import type { QuestionDtoCard } from "@/entities/question";
import { questionKeys } from "@/entities/question";

export const questionCache = {
  replace: (qc: QueryClient, updated: QuestionDtoCard) => {
    qc.setQueryData<QuestionDtoCard[]>(questionKeys.lists(), (old) =>
      old?.map((q) => (q.questionId === updated.questionId ? updated : q)),
    );
    qc.setQueryData(questionKeys.detail(updated.questionId), updated);
  },
  invalidateAll: (qc: QueryClient) =>
    qc.invalidateQueries({ queryKey: questionKeys.all }),
};