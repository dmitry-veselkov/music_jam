import type { CellDto, PackDto } from "@/entities/pack";
import type { PackRequest } from "@/features/pack";
import type { CellDraft, PackDraftState, RoundDraft } from "@/widgets/Pack/model/draft.ts";

export const emptyCell = (cost: number, category: string): CellDraft => ({
  cost,
  category,
  questionId: null,
  correctAnswer: null,
});

export const emptyRound = (name: string): RoundDraft => ({
  name: name,
  categories : ['Первая'],
  costs : [100],
  questions: [
    [
      {
        cost : 100,
        category : 'Первая',
        questionId : null,
        correctAnswer : null,
      }
    ]
  ],
});

export const toCellDraft = (cell: CellDto): CellDraft => ({
  cost: cell.cost,
  category: cell.category,
  questionId: cell.questionId,
  correctAnswer: cell.correctAnswer,
});

export const toPackRequest = (draft : PackDraftState) : PackRequest => ({
  name : draft.name,
  rounds: draft.rounds.map((round) => ({
    name : round.name,
    questions: round.questions.map((line) =>
      line.map(({ cost, category, questionId}) => ({
        cost,
        category,
        questionId,
      }))
    ),
  }))
})

export const toDraft = (pack? : PackDto) : PackDraftState => pack
  ? {
    ...pack
  }
  : {
    name : '',
    rounds : [
      emptyRound('Тур 1')
    ]
  }