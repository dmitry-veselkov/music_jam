import styles from "./StartPage.module.css"
import Header from "@/widgets/Header/ui/Header.tsx";
import Footer from "@/widgets/Footer/Footer.tsx";
import Layout from "@/shared/ui/Layout/Layout.tsx";
import HeroSection from "@/widgets/HeroSection/HeroSection.tsx";
import CreateGameSection from "@/widgets/CreateGameSection/CreateGameSection.tsx";

const StartPage = () => {
  return (
    <Layout>
      <Header />
      <main className={styles.homePage}>
        <div className={styles.pageContainer}>
          <HeroSection />
          <CreateGameSection />
        </div>
      </main>
      <Footer />
    </Layout>
  );
}

export default StartPage;