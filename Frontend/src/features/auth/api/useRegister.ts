import { useMutation } from "@tanstack/react-query";
import { authApi } from "@/features/auth/api/auth.api.ts";
import { useNavigate } from "react-router";

export const useRegister = () => {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: authApi.register,
    onSuccess: () => {
      navigate('/account', { replace: true })
    }
  })
}