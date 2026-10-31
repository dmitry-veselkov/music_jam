import type { QuestionCardState, QuestionDtoCard } from "@/entities/question";

export const toQuestionState = (questionDto : QuestionDtoCard) : QuestionCardState => ({
  questionId : questionDto.questionId,
  correctAnswer : questionDto.correctAnswer,
})