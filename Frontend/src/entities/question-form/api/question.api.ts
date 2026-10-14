import { http } from "@/shared/api/axios.ts";
import type { QuestionDto } from "@/entities/question-form";

export const questionApi = {
  getAll: async () => {
    const { data } = await http.get<QuestionDto[]>('/questions');
    return data ?? [];
  },
  getById: async (id: string) => {
    const { data } = await http.get<QuestionDto>(`/questions/${id}`);
    return data ?? [];
  }
}