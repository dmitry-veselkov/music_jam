import type { PackShort } from "@/entities/pack/models/types.ts";

export type QuestionDtoCard = {
  questionId: string
  requested: string
  correctAnswer: string
  usages: PackShort[] | null
}