import { useState } from "react";
import { ConfirmDialog } from "@/shared/ui/ConfirmDialog/ConfirmDialog";
import type { PackCardDto } from "@/entities/pack";
import { useDeletePack } from "@/features/pack";

export const DeletePackButton = ({ pack }: { pack: PackCardDto }) => {
  const [open, setOpen] = useState(false);
  const deletePack = useDeletePack();

  const handleConfirm = () => {
    deletePack.mutate(pack.packId, { onSuccess: () => setOpen(false) });
  };

  return (
    <>
      <img
        src="/Garbage.svg"
        alt="GarbagePack"
        onClick={() => setOpen(true)}
        aria-label="Удалить пак"
      />
      <ConfirmDialog
        open={open}
        title="Удалить пак?"
        description={`Пак «${pack.name}» будет удалён без возможности восстановления.`}
        isLoading={deletePack.isPending}
        onConfirm={handleConfirm}
        onCancel={() => setOpen(false)}
      />
    </>
  );
};