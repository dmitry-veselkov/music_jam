import type { PackFormData } from './schema';
import type { PackDto } from "@/entities/pack/models/types.ts";

export type PackPayload = Omit<PackDto, 'packId' | 'creationDate' | 'lastUpdateDate'>;

export const toPackPayload = (data: PackFormData): PackPayload => ({
  name: data.name,
  rounds: data.rounds.map((r) => ({
    name: r.name,
    categories: r.categories,
    costs: [...r.costs].sort((a, b) => a - b),
    questions: r.questions,
  })),
});