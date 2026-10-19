export type QuestionRequest = {
  requested: string | null
  correctAnswer: string | null
  questionTrack : File | null
  answerTrack : File | null
}