import { useQuery } from "@tanstack/react-query";
import { authApi } from "@/features/auth";

export const useCheckRole = (id : string) => {
  return useQuery({
    queryKey: ['checkRole', id],
    queryFn: () => authApi.checkRole(id),
  })
}