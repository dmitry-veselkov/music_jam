import type { PageParams } from "@/shared/api/pagination.ts";
import { useQuery } from "@tanstack/react-query";
import { gameApi } from "@/entities/game/api/game.api.ts";

export const useGames = (params : PageParams) => {
  return useQuery({
    queryKey: ['games', params],
    queryFn : () => gameApi.getAll(params),
  })
}

export const useGame = (id? : string) => {
  return useQuery({
    queryKey: ['game', id],
    queryFn : () => gameApi.getById(id!),
    enabled: !!id,
  })
}