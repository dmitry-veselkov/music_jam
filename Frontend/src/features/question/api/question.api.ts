import { type QuestionFormValues } from "@/features/question/model/schema.ts";
import { http } from "@/shared/api/axios.ts";

const toFormData = (data: QuestionFormValues) => {
  const form = new FormData();
  if (data.questionTrack.length) form.append('questionTrack', data.questionTrack[0]);
  if (data.answerTrack.length) form.append('answerTrack', data.answerTrack[0]);
  form.append('requested', data.requested);
  form.append('correctAnswer', data.correctAnswer);
  return form;
};

export const questionApi = {
  create: async (question: QuestionFormValues) => {
    const { data } = await http.post('/questions', toFormData(question))
    return data;
  },
  update: async ({ id, ...question} : QuestionFormValues & { id : string}) => {
    const { data } = await http.patch(`/questions/${id}`, toFormData(question))
    return data;
  },
  delete: async (id: string) => {
    const { data } = await http.delete(`/questions/${id}`);
    return data;
  }
}