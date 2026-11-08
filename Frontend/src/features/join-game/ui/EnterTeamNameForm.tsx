import { useState } from "react";
import styles from "./EnterTeamNameForm.module.css";
import type { Role } from "@/features/auth";

type JoinRole = Omit<Role, 'host'>

interface Props {
  onSubmit: (role: JoinRole, name: string | null) => void;
}

export const EnterTeamNameForm = ({ onSubmit }: Props) => {
  const [role, setRole] = useState<JoinRole>("player");
  const [value, setValue] = useState("");

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (role === "viewer") {
      onSubmit(role, null);
      return;
    }

    const name = value.trim();

    if (name) {
      onSubmit(role, name);
    }
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit}>
      <h2 className={styles.title}>Вход в игру</h2>

      <p className={styles.description}>
        Выберите, в каком качестве хотите присоединиться
      </p>

      <div className={styles.modeSelector}>
        <button
          type="button"
          className={`${styles.modeButton} ${
            role === "player" ? styles.active : ""
          }`}
          onClick={() => setRole("player")}
        >
          Команда
        </button>

        <button
          type="button"
          className={`${styles.modeButton} ${
            role === "viewer" ? styles.active : ""
          }`}
          onClick={() => setRole("viewer")}
        >
          Зритель
        </button>
      </div>

      {role === "player" && (
        <div className={styles.inputContainer}>
          <label htmlFor="team-name">Название команды</label>

          <input
            id="team-name"
            value={value}
            onChange={(e) => setValue(e.target.value)}
            placeholder="Введите название команды"
            autoFocus
            maxLength={30}
          />
        </div>
      )}

      {role === "viewer" && (
        <p className={styles.viewerDescription}>
          Вы присоединитесь к игре в качестве зрителя
          и сможете наблюдать за ходом игры.
        </p>
      )}

      <button
        type="submit"
        className={styles.submitButton}
        disabled={role === "Player" && !value.trim()}
      >
        Войти в зал
      </button>
    </form>
  );
};