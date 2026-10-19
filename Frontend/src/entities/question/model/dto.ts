import type { PackShort } from "@/entities/pack/models/types.ts";

export type QuestionDtoWithTracks = {
  questionId: string
  questionTrackUrl: string
  answerTrackUrl : string
  requested: string
  correctAnswer : string
}

export type QuestionDtoCard = {
  questionId: string
  requested: string
  correctAnswer: string
  usages: PackShort[] | null
}