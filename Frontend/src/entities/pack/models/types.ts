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

export type QuestionDto = {
  questionId : number;
  category_id : number;
  cost_id : number;
}