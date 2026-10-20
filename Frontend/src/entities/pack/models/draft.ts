export type PackDraftState = {
  name: string;
  rounds: RoundDraft[];
};

export type RoundDraft = {
  name: string;
  categories: string[];
  costs: number[];
  questions: CellDraft[][];
};

export type CellDraft = {
  questionId : string | null;
  correctAnswer: string | null;
  cost : number;
  category : string
}