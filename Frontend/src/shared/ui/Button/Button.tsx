import { forwardRef, type ButtonHTMLAttributes, type ReactNode } from "react";
import clsx from "clsx";
import styles from "./Button.module.css";

export type ButtonVariant = "primary" | "secondary" | "outline" | "muted";
export type ButtonSize = "sm" | "md" | "lg";
export type ButtonShape = "pill" | "rounded";

export type ButtonProps = Omit<ButtonHTMLAttributes<HTMLButtonElement>, "children"> & {
  text?: string;
  children?: ReactNode;
  variant?: ButtonVariant;
  size?: ButtonSize;
  shape?: ButtonShape;
  fullWidth?: boolean;
  loading?: boolean;
  startIcon?: ReactNode;
  endIcon?: ReactNode;
};

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      text,
      children,
      variant = "primary",
      size = "md",
      shape = "pill",
      fullWidth = false,
      loading = false,
      disabled,
      startIcon,
      endIcon,
      type = "button",
      className,
      ...rest
    },
    ref,
  ) => {
    return (
      <button
        ref={ref}
        type={type}
        className={clsx(
          styles.btn,
          styles[variant],
          styles[size],
          styles[shape],
          fullWidth && styles.fullWidth,
          loading && styles.loading,
          className,
        )}
        disabled={disabled || loading}
        aria-busy={loading || undefined}
        {...rest}
      >
        <span className={styles.inner}>
          {startIcon && <span className={styles.icon} aria-hidden="true">{startIcon}</span>}
          {children ?? text}
          {endIcon && <span className={styles.icon} aria-hidden="true">{endIcon}</span>}
        </span>
        {loading && <span className={styles.spinner} aria-hidden="true" />}
      </button>
    );
  },
);

Button.displayName = "Button";