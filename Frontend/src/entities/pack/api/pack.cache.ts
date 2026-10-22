import type { QueryClient } from "@tanstack/react-query";
import { packKeys } from "@/entities/pack";
import type { PackDto } from "@/entities/pack";

export const packCache = {
  replace: (qc: QueryClient, updated: PackDto) => {
    qc.setQueryData(packKeys.detail(updated.packId), updated);
  },
  invalidateAll: (qc: QueryClient) =>
    qc.invalidateQueries({ queryKey: packKeys.all }),
};