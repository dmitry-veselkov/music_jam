import type { PageParams } from "@/shared/api/pagination.ts";
import type { GameDto } from "@/entities/game";
import { http } from "@/shared/api/axios.ts";

export const gameApi = {
  getAll: async ({ skip = 0, take = 20 } : PageParams) => {
    const { data } = await http.get<GameDto[]>(`/games?skip=${skip}&take=${take}`)
    return data;
  },
  getById: async (id: string) => {
    const { data } = await http.get<GameDto>(`/games/${id}`)
    return data;
  }
}