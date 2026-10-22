import { http } from "@/shared/api/axios.ts";
import type { GameRequest } from "@/features/game";

export const gameMutations = {
  upsert: async ({ id, ...game }: GameRequest & { id: string }) => {
    const { data } = await http.patch(`/games/${id}`, game)
    return data;
  },
  delete: async (id : string) => {
    const { data } = await http.delete(`/games/${id}`)
    return data;
  }
}