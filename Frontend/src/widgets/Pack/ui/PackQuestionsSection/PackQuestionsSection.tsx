import styles from "./PackQuestionsSection.module.css"
import { useQuestions } from "@/entities/question-form";
import QuestionItem from "@/widgets/QuestionItem/QuestionItem.tsx";

const PackQuestionsSection = () => {
  const { data } = useQuestions();
  const items = data?.pages.flat() ?? [];

  return (
    <div>
      <h2>Созданные вопросы</h2>
      <div className={styles.questionsField}>
        {items.map((question) => (
          <QuestionItem key={question.questionId} question={question} />
        ))}
      </div>
    </div>
  );
};

export default PackQuestionsSection;