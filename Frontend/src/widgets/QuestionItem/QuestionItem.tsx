import { useState } from "react";
import { Button } from "@/shared/ui/Button/Button.tsx";
import Modal from "@/shared/ui/Modal/Modal.tsx";
import QuestionForm from "@/features/question-form/ui/QuestionForm.tsx";
import styles from "./QuestionItem.module.css"
import type { DragEvent } from 'react';
import type { QuestionDtoCard } from "@/entities/question/model/dto.ts";

type QuestionItemProps = {
  question: QuestionDtoCard;
}

const QuestionItem = ({ question } : QuestionItemProps) => {
  const [isOpen, setIsOpen] = useState(false);

  const handleDragStart = (e: DragEvent<HTMLDivElement>) => {
    e.dataTransfer.setData(
      "application/json", JSON.stringify(question));
    e.dataTransfer.effectAllowed = 'copy';
  };

  return (
    <div>
      <div
        className={styles.questionItem}
        draggable
        onDragStart={handleDragStart}
      >
        {question.correctAnswer}
        <Button onClick={() => setIsOpen(true)} text="Редактировать"/>
      </div>

      <Modal isOpen={isOpen} onClose={() => {setIsOpen(false)}} title="Редактирование вопроса">
        <QuestionForm question={question} onSuccess={() => setIsOpen(false)} />
      </Modal>
    </div>
  );
};

export default QuestionItem;