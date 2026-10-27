import styles from "./Logo.module.css"
import clsx from "clsx";
import { useNavigate } from "react-router";

interface LogoProps {
  extraClass?: string;
}

const Logo = ({extraClass = ''} : LogoProps) => {
  const navigate = useNavigate();

  return (
    <div className={clsx(styles.logoText, extraClass)}>
      <img
        src="/Logo.svg"
        alt="Logo"
        onClick={() => navigate("/")}
      />
    </div>
  );
};

export default Logo;