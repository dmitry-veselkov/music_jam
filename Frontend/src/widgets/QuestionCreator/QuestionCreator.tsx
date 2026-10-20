import styles from "./QuestionCreator.module.css"
import QuestionForm from "@/features/question-form/ui/QuestionForm.tsx";

const QuestionCreator = () => {
  return (
    <div className={styles.createForm}>
      <div className={styles.name}>Создание вопроса</div>
      <div className={styles.questionForm}>
        <QuestionForm />
      </div>
    </div>
  );
};

export default QuestionCreator;