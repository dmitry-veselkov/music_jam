import { createContext, type ReactNode, useContext } from "react";
import type { PackDraft } from "@/widgets/Pack/model/usePackDraft.ts";

const PackDraftContext = createContext<PackDraft | null>(null);

export const PackDraftProvider = ({ draft, children }: { draft: PackDraft; children: ReactNode }) => (
  <PackDraftContext.Provider value={draft}>{children}</PackDraftContext.Provider>
);

export const usePackDraftContext = () => {
  const ctx = useContext(PackDraftContext);
  if (!ctx) throw new Error("Нужен PackDraftProvider");
  return ctx;
}
