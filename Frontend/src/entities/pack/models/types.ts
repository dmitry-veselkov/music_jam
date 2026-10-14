import type { QuestionDto } from "@/entities/question-form/model/types.ts";

export type PackCardDto = {
  packId : string;
  name : string;
  creationDate : Date;
  lastUpdateDate : Date;
}

export type PackCardListDto = {
  packs : PackCardDto[];
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