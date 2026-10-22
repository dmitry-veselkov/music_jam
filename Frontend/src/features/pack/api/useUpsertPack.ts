import { useMutation } from "@tanstack/react-query";
import { queryClient } from "@/shared/api/queryClient.ts";
import { packMutations } from "@/features/pack/api/pack.mutations.ts";
import { packCache } from "@/entities/pack/api/pack.cache.ts";
import { useNavigate } from "react-router";
import { packKeys } from "@/entities/pack/api/pack.queries.ts";

export const useUpsertPack = () => {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: packMutations.upsert,
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: packKeys.detail(id) });
      packCache.invalidateAll(queryClient);
      navigate('/packs');
    },
  })
}