import type { QuestionDtoCard } from "@/entities/question";
import type { QuestionCard } from "@/entities/question/model/state.ts";

export const toQuestionState = (questionDto : QuestionDtoCard) : QuestionCard => ({
  questionId : questionDto.questionId,
  correctAnswer : questionDto.correctAnswer,
})