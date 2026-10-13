import type { QuestionDto } from "@/entities/question-form";
import { useState } from "react";
import { Button } from "@/shared/ui/Button/Button.tsx";
import Modal from "@/shared/ui/Modal/Modal.tsx";
import QuestionForm from "@/features/question/ui/QuestionForm.tsx";
import styles from "./QuestionItem.module.css"
import type { DragEvent } from 'react';

type QuestionItemProps = {
  question: QuestionDto;
}

const QuestionItem = ({ question } : QuestionItemProps) => {
  const [isOpen, setIsOpen] = useState(false);

  const handleDragStart = (e: DragEvent<HTMLDivElement>) => {
    if (!question.answerTrack) return;
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
        {question.requested}
        <Button onClick={() => { console.log('click'); setIsOpen(true)}} text="Редактировать"/>
      </div>

      <Modal isOpen={isOpen} onClose={() => {setIsOpen(false)}} title="Редактирование вопроса">
        <QuestionForm question={question} onSuccess={() => setIsOpen(false)} />
      </Modal>
    </div>
  );
};

export default QuestionItem;