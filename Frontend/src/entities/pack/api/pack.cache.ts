import type { QueryClient } from "@tanstack/react-query";
import { packKeys } from "@/entities/pack/api/pack.queries.ts";
import type { PackDto } from "@/entities/pack/models/types.ts";

export const packCache = {
  replace: (qc: QueryClient, updated: PackDto) => {
    qc.setQueryData(packKeys.detail(updated.packId), updated);
  },
  invalidateAll: (qc: QueryClient) =>
    qc.invalidateQueries({ queryKey: packKeys.all }),
};