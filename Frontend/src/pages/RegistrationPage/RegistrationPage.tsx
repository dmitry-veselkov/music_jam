import Input from "@/shared/ui/Input/Input.tsx";
import { Button } from "@/shared/ui/Button/Button.tsx";
import styles from "./RegistrationPage.module.css"
import Layout from "@/shared/ui/Layout/Layout.tsx";
import Header from "@/widgets/Header/Header.tsx";
import { type AuthRequest, authSchema } from "@/features/auth/model/schema.ts";
import { useRegister } from "@/features/auth/api/useRegister.ts";
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';

const RegistrationPage = () => {
  const { register, handleSubmit } = useForm<AuthRequest>({
    resolver : zodResolver(authSchema)
  })

  const { mutate } = useRegister();

  return (
    <Layout>
      <Header showAuthButtons={false} />
      <div className={styles.authContainer}>
        <div className={styles.authCard}>
          <h1 className={styles.textCenter}>Регистрация</h1>
          <form onSubmit={handleSubmit((data) => mutate(data))}>
            <div className={styles.formGroup}>
              <label htmlFor="reg-name">Имя Фалимия</label>
              <Input
                id="reg-name"
                placeholder="Иван Иванов"
                maxLength={250}
                {...register("nickname")}
              />
            </div>

            <div className={styles.formGroup}>
              <label htmlFor="reg-email">Email</label>
              <Input
                id="reg-email"
                type="email"
                placeholder="mail@example.com"
                maxLength={150}
                {...register("email")}
              />
            </div>

            <div className={styles.formGroup}>
              <label htmlFor="reg-password">Пароль</label>
              <Input
                id="reg-password"
                type="password"
                placeholder="••••••••"
                maxLength={100}
                {...register("password")}
              />
            </div>

            <div className={`${styles.btnWrapper} ${styles.mt3}`}>
              <Button
                id="reg-btn"
                text="Создать аккаунт"
                variant="primary"
                type="submit"  // потом будет submit
              />
            </div>

            {/*${WrongText.html}*/}

            {/*<p className={`${styles.textCenter} ${styles.textMuted} ${styles.mt3}`}>*/}
            {/*  Уже есть аккаунт?*/}
            {/*  <a href="/login" className={styles.textAccent} data-link> Войти</a>*/}
            {/*</p>*/}
          </form>
        </div>
      </div>
    </Layout>
  );
};

export default RegistrationPage;