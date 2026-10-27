import styles from "./StartPage.module.css"
import Header from "@/widgets/Header/Header.tsx";
import Footer from "@/widgets/Footer/Footer.tsx";
import Layout from "@/shared/ui/Layout/Layout.tsx";

const STEPS = [
  "Создай новую игру",
  "Составь вопросы из своих треков",
  "Пригласи друзей",
];

const StartPage = () => {
  return (
    <Layout>
      <Header />
      <main className={styles.page}>
        <section className={styles.hero}>
          <div className={styles.createGame}>
            <div className={styles.description}>
              <h1 className={styles.title}>Музыкальный свояк</h1>
              <p className={styles.subtitle}>
                Создавай свои музыкальные паки или присоединяйся к играм друзей!
              </p>
            </div>
            <a className={styles.createButton} href="/games">
              <span className={styles.plus} aria-hidden="true">+</span>
              Создать игру
            </a>
          </div>

          <form className={styles.joinCard}>
            <div className={styles.joinHeader}>
              <h2 className={styles.joinTitle}>Присоединиться к игре</h2>
              <p className={styles.joinHint}>Вводите шестизначный код</p>
            </div>

            <label className={styles.codeField}>
              <span className={styles.codeLabel}>Код комнаты</span>
              <input
                className={styles.codeInput}
                type="text"
                inputMode="numeric"
                maxLength={6}
                placeholder="000000"
              />
            </label>

            <button className={styles.joinButton} type="submit">
              Подключиться
            </button>
          </form>
        </section>

        <section className={styles.steps}>
          <h2 className={styles.stepsTitle}>Как создать свою игру</h2>
          <ol className={styles.stepsList}>
            {STEPS.map((text, i) => (
              <li key={text} className={styles.step}>
                <span className={styles.stepNumber}>{i + 1}</span>
                <span className={styles.stepText}>{text}</span>
              </li>
            ))}
          </ol>
        </section>
      </main>
      <Footer />
    </Layout>
  );
}

export default StartPage;