import styles from "./HeroSection.module.css"
import JoinGamePanel from "@/widgets/JoinGamePanel/ui/JoinGamePanel.tsx";

const HeroSection = () => {
  return (
    <section className={styles.heroSection}>
      <div className={styles.heroSectionAbout}>
        <h1>Музыкальный свояк</h1>
        <p>
          Создавай свои музыкальные паки или присоединяйся к играм друзей!
        </p>
      </div>
      <JoinGamePanel />
    </section>
  );
};

export default HeroSection;