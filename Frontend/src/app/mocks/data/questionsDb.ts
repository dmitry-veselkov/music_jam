import type { QuestionDtoCard } from "@/entities/question";

const mockAnswerTrack = new File(['mock audio content'], 'answer.mp3', { type: 'audio/mpeg' });
const mockQuestionTrack = new File(['mock audio content'], 'question.mp3', { type: 'audio/mpeg' });

export type Question = Omit<QuestionDtoCard, 'usages'> & {
  questionTrack: File;
  answerTrack: File;
};

export const questionsDb: Question[] = [
  { questionId: '1', requested: 'Название песни', correctAnswer: 'Смешарики песня', questionTrack: mockQuestionTrack, answerTrack: mockAnswerTrack },
  { questionId: '2', requested: 'Название песни', correctAnswer: 'Смешарики2 песня', questionTrack: mockQuestionTrack, answerTrack: mockAnswerTrack },
  { questionId: '3', requested: 'Название песни', correctAnswer: 'Смешарики3 песня', questionTrack: mockQuestionTrack, answerTrack: mockAnswerTrack },
];

export const toCard = (q: Question): QuestionDtoCard => ({
  questionId: q.questionId,
  requested: q.requested,
  correctAnswer: q.correctAnswer,
  usages: null,
});