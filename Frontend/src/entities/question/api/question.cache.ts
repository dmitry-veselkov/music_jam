import type { QueryClient } from "@tanstack/react-query";
import type { QuestionDto } from "@/entities/question";
import { questionKeys } from "@/entities/question/api/question.queries.ts";

export const questionCache = {
  replace: (qc: QueryClient, updated: QuestionDto) => {
    qc.setQueryData<QuestionDto[]>(questionKeys.lists(), (old) =>
      old?.map((q) => (q.questionId === updated.questionId ? updated : q)),
    );
    qc.setQueryData(questionKeys.detail(updated.questionId), updated);
  },
  invalidateAll: (qc: QueryClient) =>
    qc.invalidateQueries({ queryKey: questionKeys.all }),
};