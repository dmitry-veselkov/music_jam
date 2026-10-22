import styles from "./PackQuestionsSection.module.css"
import { useQuestions } from "@/entities/question";
import { QuestionCard } from "@/entities/question";
import { EditQuestionButton } from "@/features/question/ui/EditQuestionButton/EditQuestionButton.tsx";

const PackQuestionsSection = () => {
  const { data } = useQuestions();
  const items = data?.pages.flat() ?? [];

  return (
    <div>
      <h2>Созданные вопросы</h2>
      <div className={styles.questionsField}>
        {items.map((question) => (
          <QuestionCard
            key={question.questionId}
            question={question}
            actions={<EditQuestionButton question={question} />}
          />
        ))}
      </div>
    </div>
  );
};

export default PackQuestionsSection;