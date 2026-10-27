import type { ReactNode } from "react";
import clsx from "clsx";
import type { GameDto } from "@/entities/game";
import styles from "./GameSection.module.css";

type GameSectionProps = {
  title: string;
  games: GameDto[];
  renderCard: (game: GameDto) => ReactNode;
  variant?: "default" | "past";
};

const GameSection = ({ title, games, renderCard, variant = "default" }: GameSectionProps) => {
  if (games.length === 0) return null;

  return (
    <section className={clsx(styles.section, variant === "past" && styles.past)}>
      <h2 className={styles.title}>{title}</h2>
      <ul className={styles.list}>{games.map(renderCard)}</ul>
    </section>
  );
};

export default GameSection;