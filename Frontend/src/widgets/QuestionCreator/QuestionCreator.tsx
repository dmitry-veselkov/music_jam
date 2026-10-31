import styles from "./QuestionCreator.module.css"
import { QuestionForm } from "@/features/question";

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