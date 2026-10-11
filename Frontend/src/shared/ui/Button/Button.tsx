import styles from "./Button.module.css";
import clsx from "clsx";

type ButtonType = 'primary' | 'secondary' | "outline";

type ButtonProps = {
  text: string;
  id? : string;
  type?: 'submit' | 'button';
  variant?: ButtonType;
  loading?: boolean;
  disabled?: boolean;
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
  disabled = false,
  loading = false,
  extraClass = '',
  onClick = () => {}
} : ButtonProps) => {
  return (
    <button
      id={id}
      type={type}
      className={clsx(styles.btn, variantClass[variant], loading && styles.loading, extraClass)}
      onClick={onClick}
      disabled={disabled || loading}
    >
      <span className={styles.btnText}>{text}</span>
      <span className={styles.btnSpinner} />
    </button>
  );
};