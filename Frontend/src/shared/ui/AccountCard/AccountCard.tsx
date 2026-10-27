import { useNavigate } from "react-router";
import styles from "@/shared/ui/AccountCard/AccountCard.module.css";

const AccountCard = () => {
  const navigate = useNavigate();

  return (
    <div className={styles.accountCard}>
      <div>
        Личный кабинет
      </div>
      <img
        src="/AccountIcon.svg"
        alt="AccountIcon"
        onClick={() => navigate("/login")}
      />
    </div>
  );
};

export default AccountCard;