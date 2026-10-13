import { useQuery } from "@tanstack/react-query";
import { packApi } from "@/entities/pack/api/pack.api.ts";

export const packKeys = {
  all: ['packsCards'] as const,
  lists: () => [...packKeys.all, 'list'] as const,
  detail: (id: string) => [...packKeys.all, 'detail', id] as const,
}

export const usePacks = () => {
  return useQuery({
    queryKey: packKeys.lists(),
    queryFn: () => packApi.getAll(),
  })
}

export const usePack = (id? : string) => {
  return useQuery({
    queryKey: packKeys.detail(id!),
    queryFn: () => packApi.getById(id!),
    enabled: !!id,
  })
}