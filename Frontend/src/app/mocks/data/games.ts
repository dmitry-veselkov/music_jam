import type { GameDto } from "@/entities/game/model/types.ts";

export const games : GameDto[] = [
  {
    gameId : "1",
    pack : {
      packId : "1",
      name : "Первый"
    },
    name : "Первая",
    description : "Первая игра",
    date: '2006-01-01T00:00:00.000Z',
    mode : "withHost",
    state : "finish"
  }
]