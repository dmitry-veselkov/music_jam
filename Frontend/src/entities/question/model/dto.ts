import type { PackShort } from "@/entities/pack";

export type QuestionDtoCard = {
  questionId: string
  requested: string
  correctAnswer: string
  usages: PackShort[] | null
}