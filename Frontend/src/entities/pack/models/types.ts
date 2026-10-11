import type { QuestionDto } from "@/entities/question-form/model/types.ts";

export type PackCardDto = {
  packId : number;
  name : string;
  creationDate : Date;
  lastUpdateDate : Date;
}

export type PackCardListDto = {
  packs : PackCardDto[];
  totalCount : number;
}

export type PackDto = PackCardDto & {
  rounds : RoundDto[]
}

export type RoundDto = {
  questions : QuestionDto[]
}