import z from "zod";

const questionSchema = z.object({
  questionId: z.string(),
  questionTrackUrl: z.string(),
  answerTrackUrl: z.string(),
  requested: z.string(),
  correctAnswer: z.string(),
});

const cellSchema = z.object({
  cost: z.number().positive(),
  category: z.string(),
  question: questionSchema,
});

const roundSchema = z.object({
  name: z.string().trim().min(1, 'Введите название раунда'),
  categories: z.array(z.string().trim().min(1)).min(1, 'Нужна хотя бы одна категория'),
  costs: z.array(z.number().positive()).min(1, 'Нужна хотя бы одна стоимость'),
  questions: z.array(z.array(cellSchema)),
});

export const packSchema = z.object({
  name: z.string().trim().min(1, 'Введите название пака'),
  rounds: z.array(roundSchema).min(1, 'Нужен хотя бы один раунд'),
});

export type PackFormData = z.infer<typeof packSchema>;
export type PackErrors = ReturnType<z.ZodError<PackFormData>['flatten']>;