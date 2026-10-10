import styles from "./Button.module.css";
import clsx from "clsx";

type ButtonType = 'primary' | 'secondary' | "outline";

type ButtonProps = {
  text: string;
  id? : string;
  type?: 'submit' | 'button';
  variant?: ButtonType;
  loading?: boolean;
  extraClass?: string;
  onClick?: () => void;
}

const variantClass: Record<ButtonType, string> = {
  primary: styles.btnPrimary,
  secondary: styles.btnSecondary,
  outline: styles.btnOutline,
};

export const Button = ({
  text,
  id = '',
  type = 'button',
  variant = 'primary',
  loading = false,
  extraClass = '',
  onClick = () => {}
} : ButtonProps) => {
  return (
    <button
      id={id}
      type={type}
      className={clsx(styles.btn, variantClass[variant], loading && styles.loading, extraClass)}
      onClick={onClick}>
      <span className={styles.btnText}>{text}</span>
      <span className={styles.btnSpinner} />
    </button>
  );
};