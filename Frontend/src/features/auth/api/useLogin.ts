import { useMutation } from "@tanstack/react-query";
import { authApi } from "@/features/auth";
import { useNavigate } from "react-router";

export const useLogin = () => {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: authApi.login,
    onSuccess: () => {
      navigate('/account', { replace: true })
    }
  })
}