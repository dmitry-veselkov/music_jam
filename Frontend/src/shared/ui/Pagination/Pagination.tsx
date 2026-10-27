import styles from "./Pagination.module.css";

type PaginationProps = {
  page: number;
  totalPages: number;
  onChange: (page: number) => void;
};

const getPageItems = (current: number, total: number): (number | "…")[] => {
  const pages = [...new Set([1, 2, 3, total - 1, total, current - 1, current, current + 1])]
    .filter((p) => p >= 1 && p <= total)
    .sort((a, b) => a - b);

  const result: (number | "…")[] = [];
  pages.forEach((p, i) => {
    const prev = pages[i - 1];
    if (prev !== undefined) {
      if (p - prev === 2) result.push(prev + 1);
      else if (p - prev > 2) result.push("…");
    }
    result.push(p);
  });
  return result;
};

const Pagination = ({ page, totalPages, onChange }: PaginationProps) => {
  if (totalPages <= 1) return null;

  return (
    <nav className={styles.pagination} aria-label="Пагинация">
      <button className={styles.item} disabled={page === 1} onClick={() => onChange(page - 1)}>
        ← Назад
      </button>

      {getPageItems(page, totalPages).map((item, i) =>
        item === "…" ? (
          <span key={`gap-${i}`} className={styles.gap}>…</span>
        ) : (
          <button
            key={item}
            className={`${styles.item} ${item === page ? styles.current : ""}`}
            aria-current={item === page ? "page" : undefined}
            onClick={() => onChange(item)}
          >
            {item}
          </button>
        ),
      )}

      <button className={styles.item} disabled={page === totalPages} onClick={() => onChange(page + 1)}>
        Вперёд →
      </button>
    </nav>
  );
};

export default Pagination;