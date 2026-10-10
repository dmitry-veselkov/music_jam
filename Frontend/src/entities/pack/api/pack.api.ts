import { http } from "@/shared/api/axios.ts";
import type { PackCardListDto, PackDto } from "@/entities/pack/models/types.ts";

export const packApi = {
  getAll: async () => {
    const { data } = await http.get<PackCardListDto>('/packs')
    return data ?? [];
  },
  getById: async (id: string) => {
    const { data } = await http.get<PackDto>(`/packs/${id}`);
    return data;
  }
}