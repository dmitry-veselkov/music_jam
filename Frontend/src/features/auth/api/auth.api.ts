import type { AuthRequest, CheckRoleResponse, LoginRequest } from "@/features/auth";
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
  },
  checkRole: async (id : string) => {
    const response = await http.get<CheckRoleResponse>(`/runtime/games/${id}/users/check-role`)
    return response.data;
  },
  checkHost: async (id: string) => {
    const response = await http.get<{isHost : boolean}>(`/runtime/games/${id}/users/check-host`)
    return response.data;
  }
}
