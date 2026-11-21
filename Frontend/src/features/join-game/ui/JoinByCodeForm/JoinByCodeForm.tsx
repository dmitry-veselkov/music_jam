import { useGameByCode } from "@/entities/game";
import { type FormEvent, useState } from "react";

import styles from "./JoinByCodeForm.module.css"
import Input from "@/shared/ui/Input/Input.tsx";

const JoinByCodeForm = () => {
  const [code, setCode] = useState("");
  const [submittedCode, setSubmittedCode] = useState<string | null>(null);;
  const { isError } = useGameByCode(submittedCode ?? "");

  const handleChange = (value: string) => {
    setCode(value.toUpperCase());
  };

  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (/^[A-Z0-9]{6}$/.test(code)) {
      setSubmittedCode(code);
    }
  };

  return (
    <form
      className={styles.joinGameForm}
      onSubmit={handleSubmit}
    >
      <div className={styles.joinGameFormByCode}>
        <label className={styles.joinGameFormByCodeLabel}>
          Код комнаты
        </label>
        <Input
          className={styles.joinGameFormByCodeInput}
          name="roomCode"
          placeholder="000000"
          value={code}
          onChange={(event) => handleChange(event.target.value)}
          type="text"
          required
          minLength={6}
          maxLength={6}
          pattern="[A-Z0-9]{6}"
        />
      </div>

      <button
        className={styles.joinGameFormButton}
        type="submit"
        disabled={code.length !== 6}
      >
        Подключиться
      </button>

      {isError && <p>Не удалось найти комнату</p>}
    </form>
  );
};

export default JoinByCodeForm;