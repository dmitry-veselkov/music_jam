import { useEffect, useState } from 'react';
import type { CellDto, PackDto, PackRequest, RoundDto } from "@/entities/pack/models/types.ts";

const emptyRound = (name: string): RoundDto => ({
  name: name,
  categories: ['Первая'],
  costs: [100],
  questions: [
    [
      null
    ]
  ],
});

const toDraft = (pack?: PackDto): PackRequest =>
  pack !== undefined
    ? { name: pack.name, rounds: pack.rounds }
    : { name: '', rounds: [emptyRound("Тур 1")] };

export const usePackDraft = (initial? : PackDto) => {
  const [pack, setPack] = useState<PackRequest>(() => toDraft(initial));
  const [roundIndex, setRoundIndex] = useState(0);

  useEffect(() => {
    setPack(toDraft(initial));
    // возможно херота
    setRoundIndex(0);
  }, [initial?.packId]);

  const updateRound = (index: number, patch: Partial<RoundDto>) =>
    setPack((p) => ({
      ...p,
      rounds: p.rounds.map((r, i) => (i === index ? { ...r, ...patch } : r)),
    }));

  const setCell = (row: number, col: number, cell: CellDto | null) => {
    return setPack((p) =>({
      ...p,
      rounds: p.rounds.map((r, i) => {
        if (i !== roundIndex) return r;

        const questions = r.questions.map((line) => [...line]);
        while (questions.length <= row) questions.push([]);
        while (questions[row].length <= col) questions[row].push(null);
        questions[row][col] = cell;

        return {...r, questions};
      })
    }));
  };

  const round = pack.rounds[roundIndex];


  return {
    pack,
    round,
    roundIndex,
    setRoundIndex,
    setCell,
    setName: (name: string) => setPack((p) => ({ ...p, name })),
    addRound: (name : string) =>
      setPack((p) => {
          // const order = (pack.rounds.at(-1)?.order ?? 0) + 1;
          setRoundIndex(p.rounds.length);
          return { ...p, rounds: [...p.rounds, emptyRound(name)] };
        }),

    addCost: (cost: number) =>
      updateRound(roundIndex, { costs: [...round.costs, cost] }),

    addCategory: (name: string) =>
      updateRound(roundIndex, { categories: [...round.categories, name] }),
  };
};
export type PackDraft = ReturnType<typeof usePackDraft>;