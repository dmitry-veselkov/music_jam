import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { packApi } from "@/entities/pack/api/pack.api.ts";
import type { PageParams } from "@/shared/api/pagination.ts";

export const packKeys = {
  all: ['packsCards'] as const,
  lists: () => [...packKeys.all, 'list'] as const,
  list: (p: PageParams) => [...packKeys.lists(), p] as const,
  detail: (id: string) => [...packKeys.all, 'detail', id] as const,
}

export const usePackCardList = (params: PageParams) => {
  return useQuery({
    queryKey: packKeys.lists(),
    queryFn: () => packApi.getAll(params),
    placeholderData: keepPreviousData,
  })
}

export const usePack = (id? : string) => {
  return useQuery({
    queryKey: packKeys.detail(id!),
    queryFn: () => packApi.getById(id!),
    enabled: !!id,
  })
}

export const usePackShorts = (params: PageParams) => {
  return useQuery({
    queryKey: ['packsShort', 'list'],
    queryFn: () => packApi.getAllInfo(params),
    placeholderData: keepPreviousData,
    select: (data) =>
      data.map(({ packId, name }) => ({packId, name})),
  })
}