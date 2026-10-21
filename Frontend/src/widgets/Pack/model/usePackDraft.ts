import { useState } from 'react';
import type { PackDto } from "@/entities/pack";
import type { CellDraft, PackDraftState, RoundDraft } from "@/widgets/Pack/model/draft.ts";
import { emptyCell, emptyRound, toDraft } from "@/widgets/Pack/model/mappers.ts";

export const usePackDraft = (initial? : PackDto) => {
  const [packId] = useState(() => initial?.packId ?? crypto.randomUUID());
  const [pack, setPack] = useState<PackDraftState>(() => toDraft(initial));
  const [roundIndex, setRoundIndex] = useState(0);

  const updateRound = (index: number, update: (r: RoundDraft) => RoundDraft) =>
    setPack((p) => ({
      ...p,
      rounds: p.rounds.map((r, i) => (i === index ? update(r) : r)),
    }));

  const setCell = (row: number, col: number, cell: CellDraft) =>
    setPack((p) => ({
      ...p,
      rounds: p.rounds.map((r, i) =>
        i !== roundIndex
          ? r
          : {
            ...r,
            questions: r.questions.map((line, y) =>
              y === row ? line.map((c, x) => (x === col ? cell : c)) : line
            ),
          }
      ),
    }));

  const addRound = (name: string) => {
    setRoundIndex(pack.rounds.length);
    setPack((p) => ({ ...p, rounds: [...p.rounds, emptyRound(name)] }));
  };

  const addCost = (cost: number) =>
    updateRound(roundIndex, (r) => ({
      ...r,
      costs: [...r.costs, cost],
      questions: r.questions.map((row, i) => [...row, emptyCell(cost, r.categories[i])]),
    }));

  const addCategory = (name: string) =>
    updateRound(roundIndex, (r) => ({
      ...r,
      categories: [...r.categories, name],
      questions : [...r.questions, r.costs.map((cost) => emptyCell(cost, name))]
    }));

  const setName = (name: string) => setPack((p) => ({ ...p, name }));


  return {
    packId,
    pack,
    round : pack.rounds[roundIndex],
    roundIndex,
    setRoundIndex,
    setCell,
    setName,
    addRound,
    addCost,
    addCategory,
  };
};
export type PackDraft = ReturnType<typeof usePackDraft>;