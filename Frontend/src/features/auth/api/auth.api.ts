import type { AuthRequest, LoginRequest } from "@/features/auth/model/schema.ts";
import { http } from "@/shared/api/axios.ts";

export const authApi = {
  // временные урлы
  register: async (data: AuthRequest) => {
    const response = await http.post('/auth', data)
    return response.data;
  },
  login: async (data: LoginRequest) => {
    const response = await http.post('/auth', data)
    return response.data;
  },
  me: async () => {
    const response = await http.get('/me')
    return response.data;
  }
}
