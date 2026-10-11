import type { AuthRequest } from "@/features/auth/model/schema.ts";
import { http } from "@/shared/api/axios.ts";

export const authApi = {
  register: async (data: AuthRequest) => {
    const response = await http.post('/register', data)
    return response.data;
  }
}
