import { useMutation } from "@tanstack/react-query";
import { questionCache } from "@/entities/question-form";
import { queryClient } from "@/shared/api/queryClient.ts";
import { packApi } from "@/features/pack/api/pack.api.ts";
import { useNavigate } from "react-router";

export const useCreatePack = () => {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: packApi.create,
    onSuccess: () => {
      questionCache.invalidateAll(queryClient)
      navigate('/packs')
    },
  })
}