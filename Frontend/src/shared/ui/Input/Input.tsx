import styles from "./Input.module.css"
import type { ComponentPropsWithRef } from "react";

type InputProps = ComponentPropsWithRef<'input'>

const Input = ({
  type = 'text',
  maxLength = 10,
  ...props
} : InputProps) => {
  return (
    <input
      type={type}
      className={styles.uiInput}
      maxLength={maxLength}
      autoComplete="off"
      {...props}
    />
  );
};

export default Input;