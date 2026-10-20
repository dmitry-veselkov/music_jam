import type { QueryClient } from "@tanstack/react-query";

export const gameCache = {
  invalidateAll: (qc: QueryClient) =>
    qc.invalidateQueries({ queryKey: ['games'] }),
}