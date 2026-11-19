import styles from "./AccountPage.module.css";
import { useNavigate } from "react-router";
import Header from "@/widgets/Header/Header.tsx";

const AccountPage = () => {
  const navigate = useNavigate();

  return (
    <div>
      <Header showAuthButtons={false}/>
      <h1 className={styles.title}>Мой аккаунт</h1>
      <div className={styles.info}>
        <div
          className={styles.myPacks}
          onClick={() => navigate('/packs')}
        >
          Паки
        </div>
      </div>
    </div>
  );
};

export default AccountPage;