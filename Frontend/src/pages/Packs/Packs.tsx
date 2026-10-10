import Header from "@/widgets/Header/Header.tsx";
import styles from "./Packs.module.css"
import PackCardList from "@/widgets/PackCardList/PackCardList.tsx";

const Packs = () => {

  return (
    <div>
      <Header showAuthButtons={false}/>
      <div className={styles.packsContent}>
        <h1>Мои паки</h1>
        <PackCardList />
      </div>
    </div>
  );
};

export default Packs;