import styles from "./StartPage.module.css"
import Header from "@/widgets/Header/Header.tsx";
import Footer from "@/widgets/Footer/Footer.tsx";
import Layout from "@/shared/ui/Layout/Layout.tsx";

const StartPage = () => {
  return (
    <Layout>
      <Header />
      <main className={styles.heroSection}>
        <h1 className={styles.heroTitle}>Музыкальный свояк</h1>
        <p className={styles.heroDescription}>
          Соревнуйся в знании любимых треков вместе с друзьями!
        </p>
      </main>
      <Footer />
    </Layout>
  );
}

export default StartPage;