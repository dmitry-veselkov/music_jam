import styles from "./AudioToolBar.module.css";

type AudioToolBarProps = {
  title: string;
  isPlaying: boolean;
  currentTime: number;
  duration: number;
  disabled?: boolean;
  onToggle: () => void;
  onSeek: (time: number) => void;
};

export const AudioToolbar= ({
   title,
   isPlaying,
   currentTime,
   duration,
   disabled,
   onToggle,
   onSeek
}: AudioToolBarProps) => {
  const progress = duration > 0 ? (currentTime / duration) * 100 : 0;
  return (
    <div className={styles.root}>
      <div className={styles.top}>
        <span className={styles.title}>{title}</span>
        <button
          className={styles.button}
          onClick={onToggle}
          disabled={disabled}
          aria-label={isPlaying ? "Пауза" : "Играть"}
        >
          {isPlaying ? (
            <svg viewBox="0 0 24 24" width="28" height="28" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M8 5v14M16 5v14" strokeLinecap="round" />
            </svg>
          ) : (
            <svg viewBox="0 0 24 24" width="28" height="28" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M7 4l13 8-13 8z" strokeLinejoin="round" />
            </svg>
          )}
        </button>
      </div>

      <input
        className={styles.range}
        type="range"
        min={0}
        max={duration || 0}
        step={0.1}
        value={currentTime}
        disabled={disabled || !duration}
        onChange={(e) => onSeek(Number(e.target.value))}
        style={{ ["--progress" as string]: `${progress}%` }}
        aria-label="Прогресс"
      />
    </div>
  );
};