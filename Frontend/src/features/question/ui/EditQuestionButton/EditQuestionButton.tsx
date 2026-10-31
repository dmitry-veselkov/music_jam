import type { QuestionDtoCard } from "@/entities/question";
import { useState } from "react";
import { Button } from "@/shared/ui/Button/Button.tsx";
import Modal from "@/shared/ui/Modal/Modal.tsx";
import { QuestionForm } from "@/features/question";

export const EditQuestionButton = ({ question }: { question: QuestionDtoCard }) => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <Button onClick={() => setIsOpen(true)} text="Редактировать"/>
      <Modal isOpen={isOpen} onClose={() => {setIsOpen(false)}} title="Редактирование вопроса">
        <QuestionForm question={question} onSuccess={() => setIsOpen(false)} />
      </Modal>
    </>
  );
};