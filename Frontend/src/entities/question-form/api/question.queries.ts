import { useQuery } from "@tanstack/react-query";
import { questionApi } from "@/entities/question-form/api/question.api.ts";

export const questionKeys = {
  all: ['questions'] as const,
  lists: () => [...questionKeys.all, 'list'] as const,
  detail: (id: string) => [...questionKeys.all, 'detail', id] as const,
};

export const useQuestions = () => {
  return useQuery({
    queryKey: questionKeys.lists(),
    queryFn: () => questionApi.getAll(),
  })
}

export const useQuestion = (id: string) => {
  return useQuery({
    queryKey: questionKeys.detail(id),
    queryFn: () => questionApi.getById(id),
  });
};