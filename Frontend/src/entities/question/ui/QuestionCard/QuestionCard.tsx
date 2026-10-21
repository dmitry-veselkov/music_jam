import { type ReactNode } from "react";
import styles from "./QuestionCard.module.css"
import type { DragEvent } from 'react';
import type { QuestionDtoCard } from "@/entities/question";

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
      {actions}
    </div>
  );
};