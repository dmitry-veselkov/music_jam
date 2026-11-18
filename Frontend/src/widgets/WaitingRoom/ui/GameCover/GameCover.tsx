import styles from "./GameCover.module.css";

interface Props {
  alt?: string;
}

export const GameCover = ({
                            alt = "Обложка игры",
                          }: Props) => {
  return (
    <img
      className={styles.cover}
      src="/CoverGame.svg"
      alt={alt}
    />
  );
};