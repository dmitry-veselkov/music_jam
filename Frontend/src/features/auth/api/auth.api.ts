import type { AuthRequest, LoginRequest } from "@/features/auth/model/schema.ts";
import { http } from "@/shared/api/axios.ts";

export const authApi = {
  register: async (data: AuthRequest) => {
    const response = await http.post('/auth/register', data)
    return response.data;
  },
  login: async (data: LoginRequest) => {
    const response = await http.post('/auth/login', data)
    return response.data;
  },
  me: async () => {
    const response = await http.get('/auth/me')
    return response.data;
  }
}
