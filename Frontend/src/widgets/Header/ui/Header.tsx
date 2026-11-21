import Logo from "@/shared/ui/Logo/Logo.tsx";
import styles from "./Header.module.css";
import { useNavigate } from "react-router";
import { useMe } from "@/features/auth/api/useMe.ts";
import AccountBadge from "@/shared/ui/AccountBadge/AccountBadge.tsx";
import axios from "axios";

const navigation = [
  { label: "О проекте", href: "#about" },
  { label: "Помощь", href: "#help" },
];

const Header = () => {
  const { data : user, isPending, error } = useMe();
  const navigate = useNavigate();

  if (isPending) {
    return null;
  }

  const isUnauthorized =
    axios.isAxiosError(error) &&
    error.response?.status === 401;
  console.log(isUnauthorized, user);

  const loginBtnSettings = {
    text: 'Войти',
    id: 'login-btn',
    variant: 'outline',
    onClick: () => navigate('/login'),
  } as const;

  const registerBtnSettings = {
    text: 'Зарегистрироваться',
    id: 'register-btn',
    variant: 'secondary',
    onClick: () => navigate('/register'),
  } as const;

  return (
    <div className={styles.headerTop}>
      <header className={styles.headerContent}>
        <Logo />
        <nav className={styles.headerNavigation} aria-label="Основная навигация">
          {navigation.map((item) => (
            <a key={item.href} href={item.href}>
              {item.label}
            </a>
          ))}
        </nav>
        {isUnauthorized
          ? (
            <div className={styles.authButtons}>
              <button { ...loginBtnSettings}>Войти</button>
              <button { ...registerBtnSettings}>Зарегестрироваться</button>
            </div>
            )
          : <AccountBadge />
        }
      </header>
    </div>
  );
};

export default Header;