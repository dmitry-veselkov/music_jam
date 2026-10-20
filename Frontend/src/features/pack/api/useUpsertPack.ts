import { useMutation } from "@tanstack/react-query";
import { queryClient } from "@/shared/api/queryClient.ts";
import { packApi } from "@/features/pack/api/pack.api.ts";
import { packCache } from "@/entities/pack/api/pack.cache.ts";
import { useNavigate } from "react-router";
import { packKeys } from "@/entities/pack/api/pack.queries.ts";

export const useUpsertPack = () => {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: packApi.upsert,
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: packKeys.detail(id) });
      packCache.invalidateAll(queryClient);
      navigate('/packs');
    },
  })
}