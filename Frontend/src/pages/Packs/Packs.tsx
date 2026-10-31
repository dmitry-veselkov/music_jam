import Header from "@/widgets/Header/Header.tsx";
import styles from "./Packs.module.css"
import PackCardList from "@/widgets/PackCardList/PackCardList.tsx";
import { Button } from "@/shared/ui/Button/Button.tsx";
import { useNavigate } from "react-router";

const Packs = () => {
  const navigate = useNavigate();

  return (
    <div>
      <Header showAuthButtons={false}/>
      <div className={styles.packsContent}>
        <div className={styles.packOptions}>
          <h1>Мои паки</h1>
          <Button
            text="Создать пак"
            onClick={() => navigate(`/packs/${crypto.randomUUID()}`)}
          />
        </div>
        <PackCardList />
      </div>
    </div>
  );
};

export default Packs;