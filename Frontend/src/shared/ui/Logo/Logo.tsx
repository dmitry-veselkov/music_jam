import styles from "./Logo.module.css"
import clsx from "clsx";

interface LogoProps {
  extraClass?: string;
}

const Logo = ({extraClass = ''} : LogoProps) => {
  return (
    <div className={clsx(styles.logoText, extraClass)}>
      <img src="/Logo.svg" alt="Logo"  width="150" height="150"/>
    </div>
  );
};

export default Logo;