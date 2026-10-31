import { http } from "@/shared/api/axios.ts";
import type { PackRequest } from "@/features/pack/model/schema.ts";

export const packMutations = {
  upsert: async ({ id, ...pack }: PackRequest & { id: string }) => {
    // бэкенд ждёт плоский список ячеек в раунде, а не сетку
    const body = {
      ...pack,
      rounds: pack.rounds.map((round) => ({ ...round, questions: round.questions.flat() })),
    };
    const { data } = await http.patch(`/packs/${id}`, body)
    return data;
  },
  delete: async (id: string) => {
    const { data } = await http.delete(`/packs/${id}`);
    return data;
  }
}