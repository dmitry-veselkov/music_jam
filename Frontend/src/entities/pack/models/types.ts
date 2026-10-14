import type { QuestionDto } from "@/entities/question-form/model/types.ts";

export type PackShort = {
  packId : string;
  name : string;
}

export type PackCardDto = PackShort & {
  creationDate : Date;
  lastUpdateDate : Date;
}


export type PackDto = PackCardDto & {
  rounds : RoundDto[]
}

export type RoundDto = {
  name: string
  categories : string[]
  costs : number[]
  questions : (CellDto | null)[][]
}

export type CellDto = {
  cost: number
  category : string
  question : QuestionDto
}

export type PackRequest = Omit<PackDto, 'packId' | 'creationDate' | 'lastUpdateDate'>;