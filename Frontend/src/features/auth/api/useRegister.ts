import { useMutation } from "@tanstack/react-query";
import { authApi } from "@/features/auth";
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