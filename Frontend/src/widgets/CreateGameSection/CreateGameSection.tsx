import styles from "./CreateGameSection.module.css"

const steps = [
  "Зарегестрируйся",
  "Создай пак",
  "Начни игру и пригласи другей",
];

const CreateGameSection = () => {
  return (
    <section className={styles.createGameSection}>
      <h2 className={styles.createGameSectionTitle}>
        Как создать свою игру
      </h2>

      <ol className={styles.createGameSteps}>
        {steps.map((description, index) => (
          <li key={index} className={styles.createGameStep}>
            <span className={styles.stepNumber}>
              {index + 1}
            </span>

            <span className={styles.stepDescription}>
              {description}
            </span>
          </li>
        ))}
      </ol>
    </section>
  );
};

export default CreateGameSection;