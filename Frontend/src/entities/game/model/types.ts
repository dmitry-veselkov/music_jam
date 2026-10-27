import type { PackShort } from "@/entities/pack";

export const MODES = {
  withHost : {value : 'withHost', label : 'С ведущим'},
  withoutHost : {value : 'withoutHost', label : 'Без ведущего'}
} as const;

type Mode = keyof typeof MODES;
export const MODE_KEYS = Object.keys(MODES) as [Mode, ...Mode[]];

export const GameState = {
  Playing: "Playing",
  Finished: "Finished",
  Pending: "Pending",
  Scheduled: "Scheduled",
} as const;

export type GameState = (typeof GameState)[keyof typeof GameState];

export type GameShort = {
  gameId: string;
  name: string;
}

export type GameDto = GameShort & {
  pack : PackShort;
  description: string;
  date: string;
  mode : Mode;
  state : GameState;
}