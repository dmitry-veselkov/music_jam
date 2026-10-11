import { setupWorker } from 'msw/browser';
import { packsHandlers } from './handlers/packs.handlers.ts';
import { questionsHandlers } from "./handlers/questions.handlers.ts";

const handlers = [
  ...packsHandlers,
  ...questionsHandlers,
]

export const worker = setupWorker(...handlers);