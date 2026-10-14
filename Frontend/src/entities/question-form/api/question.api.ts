import { http } from "@/shared/api/axios.ts";
import type { QuestionDto } from "@/entities/question-form";
import type { PageParams } from "@/shared/api/pagination.ts";

export const questionApi = {
  getAll: async ({ skip = 0, take = 20 }: PageParams = {}) => {
    const { data } = await http.get<QuestionDto[]>(`/questions?skip=${skip}&take=${take}`);
    return data ?? [];
  },
  getById: async (id: string) => {
    const { data } = await http.get<QuestionDto>(`/questions/${id}`);
    return data ?? [];
  }
}