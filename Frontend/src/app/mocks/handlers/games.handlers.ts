import { delay, http, HttpResponse } from "msw";
import { paginate } from "@/app/mocks/paginate.ts";
import type { GameDto } from "@/entities/game";
import { games } from "@/app/mocks/data/games.ts";
import { packs } from "@/app/mocks/data/packs.ts";
import { gameRequestSchema } from "@/features/game";

export const gamesHandlers = [
  http.get('/api/games', async ({ request }) => {
    await delay(100);
    return HttpResponse.json<GameDto[]>(paginate(request, games));
  }),
  http.get('/api/games/:id', async ({ params }) => {
    await delay(100);
    const game = games.find((p) => p.gameId === params.id);
    if (!game) return new HttpResponse(null, { status: 404 });
    return HttpResponse.json<GameDto>(game);
  }),
  http.patch('/api/games/:id', async ({ params, request }) => {
    const id = params.id as string;

    const parsed = gameRequestSchema.safeParse(await request.json());
    if (!parsed.success) return HttpResponse.json(parsed.error.issues, { status: 400 });
    const body = parsed.data;

    const pack = packs.find((p) => p.packId === body.packId);
    if (!pack) return new HttpResponse(null, { status: 400 });

    const existing = games.find((g) => g.gameId === id);
    const saved: GameDto = {
      gameId: id,
      pack: { packId: pack.packId, name: pack.name },
      name: body.name,
      description: body.description,
      date: body.date,
      mode: body.mode,
      state: existing?.state ?? 'planned',
    };

    if (existing) Object.assign(existing, saved);
    else games.push(saved);

    await delay(300);
    return new HttpResponse(null, { status: existing ? 204 : 201 });
  }),
  http.delete('/api/games/:id', async ({ params }) => {
    const idx = games.findIndex((p) => p.gameId === params.id);
    if (idx === -1) return new HttpResponse(null, { status: 404 });

    games.splice(idx, 1);

    await delay(300);
    return new HttpResponse(null, { status: 204 });
  }),
  // http.post('/api/runtime/new-game', ({ params }) => {
  //   return HttpResponse.json(crypto.randomUUID);
  // })
]
