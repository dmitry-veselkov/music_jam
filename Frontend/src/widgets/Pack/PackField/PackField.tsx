import { useQuestions } from "@/entities/question";
import QuestionItem from "@/widgets/QuestionItem/QuestionItem.tsx";
import styles from "./PackField.module.css"

const PackField = () => {
  const { data } = useQuestions();
  const items = data ?? [];
  return (
    <div>
      <h1>Поле</h1>
      <div className={styles.questionsField}>
        {items.map((question) => (
          <QuestionItem key={question.questionId} question={question} />
        ))}
      </div>
    </div>
  );
};

export default PackField;