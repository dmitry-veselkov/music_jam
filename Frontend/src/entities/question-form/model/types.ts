export type QuestionDto = {
  questionId: string
  questionTrackUrl: string
  answerTrackUrl : string
  requested: string
  correctAnswer : string
}

export type QuestionForm = {
  requested: string
  correctAnswer: string
  questionTrack : File | null
  answerTrack : File | null
}

export type QuestionEntity = {
  questionId: string
  requested: string
  correctAnswer: string
  questionTrack : File | null
  answerTrack : File | null
}