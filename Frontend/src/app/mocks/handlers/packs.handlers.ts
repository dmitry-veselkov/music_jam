import { http, HttpResponse, delay } from 'msw';
import type { PackCardDto } from "@/entities/pack";
import { packsCards } from "@/app/mocks/data/packsCards.ts";
import type { PackDto, PackRequest } from "@/entities/pack/models/types.ts";
import { packs } from "@/app/mocks/data/packs.ts";
import { paginate } from "@/app/mocks/paginate.ts";

const toCard = (pack : PackDto) : PackCardDto => ({
  packId :  pack.packId,
  name : pack.name,
  creationDate : pack.creationDate,
  lastUpdateDate : pack.lastUpdateDate,
})

const syncCard = (pack: PackDto) => {
  const idx = packsCards.findIndex((c) => c.packId === pack.packId);
  if (idx === -1) packsCards.push(toCard(pack));
  else packsCards[idx] = { ...packsCards[idx], ...toCard(pack) };
};

export const packsHandlers = [
  http.get('/api/packs/cards', async ({ request }) => {
    await delay(100);
    return HttpResponse.json<PackCardDto[]>(paginate(request, packsCards))
  }),
  http.get('/api/packs/:id/edit', async ({ params }) => {
    await delay(100);
    const q = packs.find((pack) => pack.packId === params.id)
    return HttpResponse.json<PackDto>(q)
  }),
  http.post('/api/packs', async ({ request }) => {
    const body = (await request.json()) as PackRequest;

    const created: PackDto = {
      ...body,
      packId: crypto.randomUUID(),
      creationDate : new Date(),
      lastUpdateDate: new Date(),
    };
    packs.push(created);
    syncCard(created);

    await delay(300);
    return HttpResponse.json({ id: created.packId }, { status: 201 });
  }),
  http.patch('/api/packs/:id', async ({ params, request }) => {
    console.log("Зашел ебать")
    const pack = packs.find((p) => p.packId === params.id);
    if (!pack) return new HttpResponse(null, { status: 404 });

    const body = (await request.json()) as Partial<PackRequest>;
    if (body.name !== undefined) pack.name = body.name;
    if (body.rounds !== undefined) pack.rounds = body.rounds;
    syncCard(pack);

    await delay(300);
    return HttpResponse.json<PackDto>(pack);
  }),
  http.delete('/api/packs/:id', async ({ params }) => {
    const idx = packs.findIndex((p) => p.packId === params.id);
    if (idx === -1) return new HttpResponse(null, { status: 404 });

    packs.splice(idx, 1);
    const cardIdx = packsCards.findIndex((c) => c.packId === params.id);
    if (cardIdx !== -1) packsCards.splice(cardIdx, 1);

    await delay(300);
    return new HttpResponse(null, { status: 204 });
  }),
]