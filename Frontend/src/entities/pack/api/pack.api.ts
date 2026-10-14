import { http } from "@/shared/api/axios.ts";
import type { PackCardDto, PackDto, PackShort } from "@/entities/pack/models/types.ts";
import type { PageParams } from "@/shared/api/pagination.ts";

export const packApi = {
  getAll: async ({ skip = 0, take = 20 }: PageParams = {}) => {
    const { data } = await http.get<PackCardDto[]>(`/packs/cards?skip=${skip}&take=${take}`)
    return data;
  },
  getById: async (id: string) => {
    const { data } = await http.get<PackDto>(`/packs/${id}/edit`);
    return data;
  },
  getByInfo: async () => {
    const { data } = await http.get<PackShort>(`/packs/short`);
    return data;
  }
}