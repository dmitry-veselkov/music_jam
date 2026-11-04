import { type ReactNode } from "react";
import styles from "./QuestionCard.module.css"
import type { DragEvent } from 'react';
import type { QuestionDtoCard } from "@/entities/question";
import { SongPlayer } from "@/features/song/ui/SongPlayer.tsx";
import { getAnswerSongUrl, getQuestionSongUrl } from "@/entities/song/model/getSongUrl.ts";

type QuestionItemProps = {
  question: QuestionDtoCard;
  actions?: ReactNode;
}

export const QuestionCard = ({ question, actions } : QuestionItemProps) => {
  const handleDragStart = (e: DragEvent<HTMLDivElement>) => {
    e.dataTransfer.setData(
      "application/json", JSON.stringify(question));
    e.dataTransfer.effectAllowed = 'copy';
  };

  return (
    <div className={styles.questionItem} draggable onDragStart={handleDragStart}>
      {question.correctAnswer}
      <SongPlayer title="Вопрос" songUrl={getQuestionSongUrl(question.questionId)} />
      <SongPlayer title="Ответ" songUrl={getAnswerSongUrl(question.questionId)} />
      {actions}
    </div>
  );
};