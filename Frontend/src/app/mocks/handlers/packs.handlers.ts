import { http, HttpResponse, delay } from 'msw';
import type { PackCardListDto } from "@/entities/pack";
import { packs } from "@/app/mocks/data/packs.ts";

export const packsHandlers = [
  http.get('/api/packs', async () => {
    await delay(100);
    return HttpResponse.json<PackCardListDto>(packs)
  })
]