import { PackCard, usePackCardList } from "@/entities/pack";
import styles from "./PackCardList.module.css";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { DeletePackButton } from "@/features/pack/ui/DeletePackButton/DeletePackButton.tsx";
import Pagination from "@/shared/ui/Pagination/Pagination.tsx";

const PAGE_SIZE = 12;

const PackCardList = () => {
  const [page, setPage] = useState(0);
  const navigate = useNavigate();
  const { data } = usePackCardList({ skip: page * PAGE_SIZE, take: PAGE_SIZE });

  useEffect(() => {
    if (data && data.length === 0 && page > 0) setPage((p) => p - 1);
  }, [data, page]);

  const items = data ?? []
  const totalPages = Math.ceil((items.length ?? 0) / PAGE_SIZE);

  return (
    <section className={styles.list}>
      <ul className={styles.packList}>
        {items.map((pack) => (
          <PackCard
            key={pack.packId}
            pack={pack}
            onClick={() => navigate(`/packs/${pack.packId}`)}
            actions={<DeletePackButton pack={pack} />}
          />
        ))}
      </ul>
      <Pagination page={page + 1} totalPages={totalPages} onChange={(p) => setPage(p - 1)} />
    </section>
  );
};

export default PackCardList;