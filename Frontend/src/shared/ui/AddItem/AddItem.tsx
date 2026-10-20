import { useState, type KeyboardEvent } from 'react';
import styles from './AddItem.module.css';

interface AddItemProps {
  label: string;
  inputType?: 'text' | 'number';
  onSubmit: (value: string) => void;
}

export const AddItem = ({ label, inputType = 'text', onSubmit }: AddItemProps) => {
  const [isEditing, setIsEditing] = useState(false);
  const [value, setValue] = useState('');

  const close = () => {
    setIsEditing(false);
    setValue('');
  };

  const submit = () => {
    const trimmed = value.trim();
    if (trimmed) onSubmit(trimmed);
    close();
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') submit();
    if (e.key === 'Escape') close();
  };

  if (!isEditing) {
    return (
      <button
        type="button"
        className={styles.addButton}
        title={label}
        aria-label={label}
        onClick={() => setIsEditing(true)}
      >
        +
      </button>
    );
  }

  return (
    <input
      autoFocus
      className={styles.input}
      type={inputType}
      value={value}
      placeholder={label}
      aria-label={label}
      onChange={(e) => setValue(e.target.value)}
      onKeyDown={handleKeyDown}
      onBlur={close}
    />
  );
};