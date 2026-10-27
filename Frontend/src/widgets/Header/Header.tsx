import { Button } from "@/shared/ui/Button/Button.tsx";
import Logo from "@/shared/ui/Logo/Logo.tsx";
import styles from "./Header.module.css";
import { useNavigate } from "react-router";

type HeaderProps = {
  showAuthButtons?: boolean;
}

const NAV_LINKS = [
  { text: "Как играть", href: "#" },
  { text: "О проекте", href: "#" },
  { text: "Помощь", href: "#" },
];

const Header = ({ showAuthButtons = true } : HeaderProps) => {
  const navigate = useNavigate();

  const loginBtnSettings = {
    text: "Вход",
    id: "login-btn",
    variant: "outline",
    onClick: () => navigate("/login"),
  } as const;

  const registerBtnSettings = {
    text: "Регистрация",
    id: "register-btn",
    variant: "secondary",
    onClick: () => navigate("/register"),
  } as const;

  return (
    <div className={styles.headerTop}>
      <header className={styles.headerContent}>
        <Logo />
        <nav className={styles.info}>
          {NAV_LINKS.map(({ text, href }) => (
            <a key={text} className={styles.link} href={href}>
              {text}
            </a>
          ))}
        </nav>
        {showAuthButtons && (
          <div className={styles.authButtons}>
            <Button {...loginBtnSettings} />
            <Button {...registerBtnSettings} />
          </div>
        )}
        {/*<AccountCard />*/}
      </header>
    </div>
  );
};

export default Header;