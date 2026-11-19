import { useQuery } from "@tanstack/react-query";
import { authApi } from "@/features/auth";

export const useCheckHost = (id : string) => {
  return useQuery({
    queryKey: ['checkHost', id],
    queryFn: () => authApi.checkHost(id),
    select: (data) => data.isHost,
  })
}