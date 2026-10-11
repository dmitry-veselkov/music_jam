import { useQuery } from "@tanstack/react-query";
import { packApi } from "@/entities/pack/api/pack.api.ts";
import { questionKeys } from "@/entities/question/api/question.queries.ts";

export const packKeys = {
  all: ['packs'] as const,
  lists: () => [...questionKeys.all, 'list'] as const,
  detail: (id: string) => [...questionKeys.all, 'detail', id] as const,
}

export const usePacks = () => {
  return useQuery({
    queryKey: packKeys.lists(),
    queryFn: () => packApi.getAll(),
  })
}

export const usePack = (id : string) => {
  return useQuery({
    queryKey: packKeys.lists(),
    queryFn: () => packApi.getById(id),
  })
}