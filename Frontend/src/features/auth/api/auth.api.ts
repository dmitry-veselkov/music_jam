import type { AuthRequest } from "@/features/auth/model/schema.ts";
import { http } from "@/shared/api/axios.ts";

export const register = async (data : AuthRequest) => {
  // todo Сделать норм ответ
  const response = await http.post<string>('/register', data)
  return response.data;
}
