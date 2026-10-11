import { useForm } from "react-hook-form";
import { getQuestionSchema, type QuestionFormValues } from '../model/schema';
import { zodResolver } from "@hookform/resolvers/zod";
import Input from "@/shared/ui/Input/Input.tsx";
import { Button } from "@/shared/ui/Button/Button.tsx";
import styles from "./QuestionForm.module.css";
import type { QuestionDto } from "@/entities/question-form/model/types.ts";
import { useCreateQuestion } from "@/features/question/api/useCreateQuestion.ts";
import { useUpdateQuestion } from "@/features/question/api/useUpdateQuestion.ts";
import { useDeleteQuestion } from "@/features/question/api/useDeleteQuestion.ts";

type QuestionFormProps = {
  question?: QuestionDto;
  onSuccess?: () => void;
}

const QuestionForm = ( { question, onSuccess } : QuestionFormProps) => {
  const isEdit = !!question;
  const { register, handleSubmit, reset, formState : { errors } } = useForm<QuestionFormValues>({
    resolver: zodResolver(getQuestionSchema(isEdit)),
    defaultValues: {
      requested: question?.requested ?? '',
      correctAnswer: question?.correctAnswer ?? '',
    },
  })
  const create = useCreateQuestion();
  const update = useUpdateQuestion();
  const deleteFn = useDeleteQuestion();

  const isPending = create.isPending || update.isPending;

  const onSubmit = (data : QuestionFormValues) => {
    if (isEdit) {
      update.mutate({ id : question.questionId, ...data}, {
        onSuccess: () => onSuccess?.()
      })
    } else {
      create.mutate(data, {
        onSuccess: () => {
          reset();
          onSuccess?.();
        },
      })
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div className={styles.form}>
        <div>
          <label htmlFor="q-track">Трек вопроса</label>
          <Input id="q-track" type="file" accept="audio/*" {...register('questionTrack')} />
          {errors.questionTrack && <span>{errors.questionTrack.message}</span>}
        </div>

        <div>
          <label htmlFor="a-track">Трек ответа</label>
          <Input id="a-track" type="file" accept="audio/*" {...register('answerTrack')} />
          {errors.answerTrack && <span>{errors.answerTrack.message}</span>}
        </div>

        <div>
          <label htmlFor="q-title">Что назвать нужно</label>
          <Input id="q-requested" placeholder="Что нужно сказать" {...register('requested')} />
          {errors.requested && <span>{errors.requested.message}</span>}
        </div>

        <div>
          <label htmlFor="q-answer">Правильный ответ</label>
          <Input id="q-answer" placeholder="Ответ" {...register('correctAnswer')} />
          {errors.correctAnswer && <span>{errors.correctAnswer.message}</span>}
        </div>
        <div className={styles.options}>
          <Button
            id="q-submit"
            disabled={isPending}
            text={isEdit ? "Сохранить вопрос" : "Создать вопрос"}
            variant="primary"
            type="submit"
          />
          {isEdit && (
            <Button
              text="🗑️"
              onClick={() => deleteFn.mutate(question.questionId, {
                onSuccess: () => onSuccess?.()
              })}
            />
          )}
        </div>
      </div>
    </form>
  );
};

export default QuestionForm;