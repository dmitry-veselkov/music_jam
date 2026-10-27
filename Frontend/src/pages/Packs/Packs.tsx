import Header from "@/widgets/Header/Header.tsx";
import styles from "./Packs.module.css"
import PackCardList from "@/widgets/PackCardList/PackCardList.tsx";
import { Button } from "@/shared/ui/Button/Button.tsx";
import { useNavigate } from "react-router";
import Layout from "@/shared/ui/Layout/Layout.tsx";

const Packs = () => {
  const navigate = useNavigate();

  return (
    <Layout>
      <Header showAuthButtons={false}/>
      <div className={styles.packsContent}>
        <div className={styles.packOptions}>
          <h1 className={styles.title}>Мои паки</h1>
          <Button
            startIcon="+"
            onClick={() => navigate(`/packs/${crypto.randomUUID()}`)}
          >Создать новый пак</Button>
        </div>
        <PackCardList />
      </div>
    </Layout>
  );
};

export default Packs;