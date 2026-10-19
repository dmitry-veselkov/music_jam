import { z } from 'zod';
import { MODE_KEYS } from "@/entities/game/model/types.ts";

export const gameSchema = z.object({
  packId: z.string().min(1, 'Выберите пак'),
  name: z.string().trim().min(1, 'Введите название'),
  description: z.string().trim().min(1, 'Введите описание'),
  date: z.string().min(1, 'Выберите дату'),
  time: z.string().min(1, 'Выберите время'),
  mode: z.enum(MODE_KEYS),
});

export type GameFormValues = z.infer<typeof gameSchema>;

export const gameRequestSchema = z.object({
  packId: z.string().min(1),
  name: z.string().trim().min(1),
  description: z.string().trim().min(1),
  date: z.iso.datetime(),
  mode: z.enum(MODE_KEYS),
});

export type GameRequest = z.infer<typeof gameRequestSchema>;