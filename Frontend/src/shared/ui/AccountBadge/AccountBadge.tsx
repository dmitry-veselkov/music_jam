import styles from "./AccountBadge.module.css"
import { useNavigate } from "react-router";

const AccountBadge = () => {
  const navigate = useNavigate();

  return (
    <div className={styles.profileBadge} onClick={() => navigate("/account")}>
      <span className={styles.profileBadgeText}>Личный кабинет</span>
      <span className={styles.profileAvatar}>
        <img src="/IconAccount.svg" alt="AccountBadge" />
      </span>
    </div>
  );
};

export default AccountBadge;