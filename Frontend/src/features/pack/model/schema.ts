import z from "zod";

const cellSchema = z.object({
  cost: z.number().positive(),
  category: z.string(),
  questionId: z.string().nullable(),
});

const roundSchema = z.object({
  name: z.string().trim().min(1, 'Введите название раунда'),
  questions: z.array(z.array(cellSchema)),
});

export const packSchema = z.object({
  name: z.string().trim().min(1, 'Введите название пака'),
  rounds: z.array(roundSchema).min(1, 'Нужен хотя бы один раунд'),
});

export type CellRequest = z.infer<typeof cellSchema>;
export type RoundRequest = z.infer<typeof roundSchema>;
export type PackRequest = z.infer<typeof packSchema>;