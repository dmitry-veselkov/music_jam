import { z } from 'zod';

const fileSchema = z.instanceof(FileList);

export const getQuestionSchema  =  (isEdit : boolean) =>
  z.object({
    questionTrack: fileSchema.refine(
      (f) => isEdit || f.length > 0,
      "Выберите файл"
    ),
    answerTrack: fileSchema.refine(
      (f) => isEdit || f.length > 0,
      "Выберите файл"
    ),
  requested: z.string().min(1, 'Введите что нужно назвать'),
  correctAnswer: z.string().min(1, 'Введите ответ'),
  });

export type QuestionFormValues = z.infer<ReturnType<typeof getQuestionSchema>>;