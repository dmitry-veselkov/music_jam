import styles from "./PackQuestions.module.css"
import QuestionForm from "@/features/question/ui/QuestionForm.tsx";

const PackQuestions = () => {
  return (
    <div className={styles.createForm}>
      <div className={styles.name}>Создание вопроса</div>
      <div className={styles.questionForm}>
        <QuestionForm />
      </div>
    </div>
  );
};

export default PackQuestions;