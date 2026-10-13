import type { PackCardDto } from "@/entities/pack";
import styles from "./PackCard.module.css";
import { useNavigate } from "react-router";
import { useDeletePack } from "@/features/pack/api/useDeletePack.ts";
import { useState } from "react";
import { ConfirmDialog } from "@/shared/ui/ConfirmDialog/ConfirmDialog.tsx";

export const PackCard = (info : PackCardDto) => {
  const navigate = useNavigate();
  const deletePack = useDeletePack();
  const [packToDelete, setPackToDelete] = useState<{ packId: string; name: string } | null>(null);

  const handleConfirm = () => {
    if (!packToDelete) return;
    deletePack.mutate(packToDelete.packId, {
      onSuccess: () => setPackToDelete(null),
    });
  };

  return (
    <div
      className={styles.packCard}
      onClick={() => navigate(`/packs/${info.packId}`)}
    >
      <div
        className={styles.deleteIcon}
        onClick={(e) => {
          e.stopPropagation();
          setPackToDelete(info)
        }}
      >
        🗑️
      </div>
      <h2 className={styles.name}>{info.name}</h2>
      <div className={styles.dates}>
        <time className={styles.date}>
          <span>Обновлён:</span>
          {new Date(info.lastUpdateDate).toLocaleDateString("ru-RU")}
        </time>
        <time className={styles.date}>
          <span>Создан:</span>
          {new Date(info.creationDate).toLocaleDateString("ru-RU")}
        </time>
      </div>
      <ConfirmDialog
        open={packToDelete !== null}
        title="Удалить пак?"
        description={`Пак «${packToDelete?.name ?? ""}» будет удалён без возможности восстановления.`}
        isLoading={deletePack.isPending}
        onConfirm={handleConfirm}
        onCancel={() => setPackToDelete(null)}
      />
    </div>
  );
};
