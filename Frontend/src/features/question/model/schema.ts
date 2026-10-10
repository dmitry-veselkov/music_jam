import { z } from 'zod';

const fileSchema = z
  .instanceof(FileList)
  .refine((files) => files.length > 0, 'Выберите файл');

export const questionSchema = z.object({
  questionTrack: fileSchema,
  answerTrack: fileSchema,
  title: z.string().min(1, 'Введите название'),
  correctAnswer: z.string().min(1, 'Введите ответ'),
});

export type QuestionRequest = z.infer<typeof questionSchema>;