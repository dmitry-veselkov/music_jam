import type { PackRequest } from "@/entities/pack/models/types.ts";
import { http } from "@/shared/api/axios.ts";

export const packApi = {
  create: async (pack : PackRequest) => {
    const { data } = await http.post('/packs', pack)
    return data;
  },
  update: async ({ id, ...pack} : PackRequest & { id : string}) => {
    const { data } = await http.patch(`/packs/${id}`, pack)
    return data;
  },
  delete: async (id: string) => {
    const { data } = await http.delete(`/packs/${id}`);
    return data;
  }
}