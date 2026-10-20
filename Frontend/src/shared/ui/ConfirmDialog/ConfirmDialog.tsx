import { useEffect, useRef } from "react";
import styles from "./ConfirmDialog.module.css";
import { Button } from "@/shared/ui/Button/Button.tsx";

type ConfirmDialogProps = {
  open: boolean;
  title: string;
  description?: string;
  confirmText?: string;
  cancelText?: string;
  isLoading?: boolean;
  onConfirm: () => void;
  onCancel: () => void;
};

export const ConfirmDialog = ({
                                open,
                                title,
                                description,
                                confirmText = "Удалить",
                                cancelText = "Отмена",
                                isLoading = false,
                                onConfirm,
                                onCancel,
                              }: ConfirmDialogProps) => {
  const ref = useRef<HTMLDialogElement>(null);

  useEffect(() => {
    const dialog = ref.current;
    if (!dialog) return;
    if (open && !dialog.open) dialog.showModal();
    if (!open && dialog.open) dialog.close();
  }, [open]);

  return (
    <dialog
      ref={ref}
      className={styles.dialog}
      onClick={(e) => {
        e.stopPropagation();
        if (e.target === ref.current) onCancel();
      }}
      onCancel={(e) => { e.preventDefault(); onCancel(); }}
    >
      <h3 className={styles.title}>{title}</h3>
      {description && <p className={styles.description}>{description}</p>}
      <div className={styles.actions}>
        <Button type="button" text={cancelText} onClick={onCancel} />
        <Button
          type="button"
          text={isLoading ? "Удаление..." : confirmText}
          onClick={onConfirm}
        />
      </div>
    </dialog>
  );
};