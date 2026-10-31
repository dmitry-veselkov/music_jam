import { useInfiniteQuery, useQuery } from "@tanstack/react-query";
import { questionApi } from "@/entities/question";

const QUESTIONS_PAGE = 20;

export const questionKeys = {
  all: ['questions'] as const,
  lists: () => [...questionKeys.all, 'list'] as const,
  infinite: () => [...questionKeys.lists(), 'infinite'] as const,
  detail: (id: string) => [...questionKeys.all, 'detail', id] as const,
};

export const useQuestions = () =>
  useInfiniteQuery({
    queryKey: questionKeys.infinite(),
    queryFn: ({ pageParam }) => questionApi.getAll({ skip: pageParam, take: QUESTIONS_PAGE }),
    initialPageParam: 0,
    getNextPageParam: (lastPage, allPages) =>
      lastPage.length < QUESTIONS_PAGE ? undefined : allPages.length * QUESTIONS_PAGE,
  });

export const useQuestion = (id: string) => {
  return useQuery({
    queryKey: questionKeys.detail(id),
    queryFn: () => questionApi.getById(id),
  });
};