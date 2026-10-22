import { http } from "@/shared/api/axios.ts";
import type { PackRequest } from "@/features/pack/model/schema.ts";

export const packMutations = {
  upsert: async ({ id, ...pack }: PackRequest & { id: string }) => {
    const { data } = await http.patch(`/packs/${id}`, pack)
    return data;
  },
  delete: async (id: string) => {
    const { data } = await http.delete(`/packs/${id}`);
    return data;
  }
}