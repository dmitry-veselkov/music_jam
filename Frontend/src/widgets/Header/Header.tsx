import { Button } from "@/shared/ui/Button/Button.tsx";
import Logo from "@/shared/ui/Logo/Logo.tsx";
import styles from "./Header.module.css";
import { useNavigate } from "react-router";

type HeaderProps = {
  showAuthButtons?: boolean;
}

const Header = ({ showAuthButtons = true } : HeaderProps) => {
  const navigate = useNavigate();

  const loginBtnSettings = {text: 'Вход', id: 'login-btn', variant: 'outline', onClick : () => navigate("/login")} as const;
  const registerBtnSettings = {text: 'Регистрация', id: 'register-btn', variant: 'secondary', onClick : () => navigate("/register")} as const;

  return (
    <div className={styles.headerTop}>
      <header className={styles.headerContent}>
        <Logo />
        {showAuthButtons && (
          <div className={styles.authButtons}>
            <Button { ...loginBtnSettings} />
            <Button { ...registerBtnSettings}/>
          </div>
        )}
      </header>
    </div>
  );
};

export default Header;