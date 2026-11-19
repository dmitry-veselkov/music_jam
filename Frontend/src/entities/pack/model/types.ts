export type PackShort = {
  packId : string;
  name : string;
}

export type PackCardDto = PackShort & {
  createdAtUtc : string;
  updatedAtUtc : string;
  // usages : PackShort[]
}

export type PackDto = PackShort & {
  rounds : RoundDto[]
}

export type RoundDto = {
  name: string
  categories : string[]
  costs : number[]
  questions : CellDto[][]
}

export type CellDto = {
  cost: number
  category : string
  correctAnswer : string | null
  questionId : string | null
}