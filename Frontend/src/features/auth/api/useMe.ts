import { useQuery } from "@tanstack/react-query";
import { authApi } from "@/features/auth";

export const useMe = () => {
  return useQuery({
    queryKey: ["auth", "me"],
    queryFn: authApi.me,
    retry: false,
  })
}