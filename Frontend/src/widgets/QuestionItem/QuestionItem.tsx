import type { QuestionDto } from "@/entities/question-form";
import { useEffect, useState } from "react";
import { Button } from "@/shared/ui/Button/Button.tsx";
import Modal from "@/shared/ui/Modal/Modal.tsx";
import QuestionForm from "@/features/question/ui/QuestionForm.tsx";
import styles from "./QuestionItem.module.css"

type QuestionItemProps = {
  question: QuestionDto;
}

const QuestionItem = ({ question } : QuestionItemProps) => {
  const [isOpen, setIsOpen] = useState(false);
  useEffect(() => {
    console.log(isOpen);
  }, [ isOpen ])
  return (
    <div>
      <div className={styles.questionItem}>
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