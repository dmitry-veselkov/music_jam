import { useQuery } from "@tanstack/react-query";
import { packApi } from "@/entities/pack/api/pack.api.ts";

export const usePacks = () => {
  return useQuery({
    queryKey: ['packs'],
    queryFn: packApi.getAll,
  })
}