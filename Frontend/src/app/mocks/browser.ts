import { setupWorker } from 'msw/browser';
import { packsHandlers } from './handlers/packs.handlers.ts';
import { questionsHandlers } from "./handlers/questions.handlers.ts";
import { gamesHandlers } from "@/app/mocks/handlers/games.handlers.ts";

const handlers = [
  ...packsHandlers,
  ...questionsHandlers,
  ...gamesHandlers,
]

export const worker = setupWorker(...handlers);