import { http, HttpResponse, delay } from 'msw';
import type { PackCardListDto } from "@/entities/pack";
import { packs } from "@/app/mocks/data/packs.ts";

export const handlers = [
  http.get('/api/packs', async () => {
    await delay(100);
    console.log(typeof packs.packs[0].creationDate);
    console.log(HttpResponse.json<PackCardListDto>(packs))
    return HttpResponse.json<PackCardListDto>(packs)
  })
]