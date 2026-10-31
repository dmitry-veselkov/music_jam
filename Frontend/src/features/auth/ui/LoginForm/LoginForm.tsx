import styles from "./Login.module.css"
import Input from "@/shared/ui/Input/Input.tsx";
import { Button } from "@/shared/ui/Button/Button.tsx";
import { useForm } from "react-hook-form";
import { type LoginRequest, loginSchema } from "@/features/auth";
import { zodResolver } from "@hookform/resolvers/zod";
import { useLogin } from "@/features/auth";

const LoginForm = () => {
  const { register, handleSubmit } = useForm<LoginRequest>({
    resolver: zodResolver(loginSchema)
  })

  const { mutate } = useLogin();

  return (
    <div className={styles.authContainer}>
      <div className={styles.authCard}>
        <h1 className={styles.textCenter}>Вход в систему</h1>
        <form onSubmit={handleSubmit((data) => mutate(data))}>
          <div className={styles.formGroup}>
            <label htmlFor="login-email">Email</label>
            <Input
              id="login-email"
              type="email"
              placeholder="mail@example.com"
              maxLength={150}
              {...register("email")}
            />
          </div>

          <div className={styles.formGroup}>
            <label htmlFor="login-password">Пароль</label>
            <Input
              id="login-password"
              type="password"
              placeholder="••••••••"
              maxLength={100}
              {...register("password")}
            />
          </div>

          <div className={`${styles.btnWrapper} ${styles.mt3}`}>
            <Button
              id="login-btn"
              text="Войти"
              variant="primary"
              type="submit"
            />
          </div>

          {/*${WrongText.html}*/}

          <p className={`${styles.textCenter} ${styles.textMuted} ${styles.mt3}`}>
            Нет аккаунта?
            <a href="/register" className={styles.textAccent} data-link> Зарегистрироваться</a>
          </p>
        </form>
      </div>
    </div>
  );
};

export default LoginForm;