import type { QuestionEntity } from "@/entities/question-form/model/types.ts";

const mockAnswerTrack = new File(
  ['mock audio content'],
  'answer.mp3',
  { type : "audio/mpeg" },
)
const mockQuestionTrack = new File(
  ['mock audio content'],
  'question.mp3',
  { type : "audio/mpeg" },
)

export const questions: QuestionEntity[] = [
  {
    questionId: "1",
    requested: 'Название песни',
    correctAnswer: 'Смешарики песня',
    questionTrack: mockQuestionTrack,
    answerTrack: mockAnswerTrack,
  },
  {
    questionId: "2",
    requested: 'Название песни',
    correctAnswer: 'Смешарики песня',
    questionTrack: mockQuestionTrack,
    answerTrack: mockAnswerTrack,
  },
  {
    questionId: "3",
    requested: 'Название песни',
    correctAnswer: 'Смешарики песня',
    questionTrack: mockQuestionTrack,
    answerTrack: mockAnswerTrack,
  },
  {
    questionId: "4",
    requested: 'Название песни',
    correctAnswer: 'Смешарики песня',
    questionTrack: mockQuestionTrack,
    answerTrack: mockAnswerTrack,
  },
  {
    questionId: "5",
    requested: 'Название песни',
    correctAnswer: 'Смешарики песня',
    questionTrack: mockQuestionTrack,
    answerTrack: mockAnswerTrack,
  },
];