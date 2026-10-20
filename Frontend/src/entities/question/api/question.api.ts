import { http } from "@/shared/api/axios.ts";
import type { PageParams } from "@/shared/api/pagination.ts";
import type { QuestionDtoCard } from "@/entities/question";

export const questionApi = {
  getAll: async ({ skip = 0, take = 20 }: PageParams = {}) => {
    const { data } = await http.get<QuestionDtoCard[]>(`/questions?skip=${skip}&take=${take}`);
    return data ?? [];
  },
  getById: async (id: string) => {
    const { data } = await http.get<QuestionDtoCard>(`/questions/${id}`);
    return data ?? [];
  }
}