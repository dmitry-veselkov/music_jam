import { useMutation } from "@tanstack/react-query";
import { queryClient } from "@/shared/api/queryClient.ts";
import { packApi } from "@/features/pack/api/pack.api.ts";
import { packCache } from "@/entities/pack/api/pack.cache.ts";
import { useNavigate } from "react-router";

export const useUpdatePack = () => {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: packApi.update,
    onSuccess: (updated) => {
      packCache.replace(queryClient, updated)
      packCache.invalidateAll(queryClient)
      navigate('/packs')
    },
  })
}