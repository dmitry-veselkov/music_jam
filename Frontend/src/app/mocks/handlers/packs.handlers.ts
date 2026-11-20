import { http, HttpResponse, delay } from 'msw';
import type { PackCardDto } from "@/entities/pack";
import { packsCards } from "@/app/mocks/data/packsCards.ts";
import type { PackDto, PackShort, RoundDto } from "@/entities/pack";
import { packs } from "@/app/mocks/data/packs.ts";
import { paginate } from "@/app/mocks/paginate.ts";
import type { PackRequest, RoundRequest } from "@/features/pack";
import { questionsDb } from "@/app/mocks/data/questionsDb.ts";

const toCard = (pack: PackDto, createdAtUtc = new Date().toISOString()): PackCardDto => ({
  packId: pack.packId,
  name: pack.name,
  createdAtUtc,
  updatedAtUtc: new Date().toISOString(),
});

const syncCard = (pack: PackDto) => {
  const idx = packsCards.findIndex((c) => c.packId === pack.packId);
  if (idx === -1) packsCards.push(toCard(pack));
  else packsCards[idx] = { ...packsCards[idx], ...toCard(pack, packsCards[idx].createdAtUtc) };
};

const toRoundDto = (round: RoundRequest): RoundDto => ({
  name: round.name,
  categories: round.questions.map((row) => row[0]?.category ?? ''),
  costs: (round.questions[0] ?? []).map((cell) => cell.cost),
  questions: round.questions.map((row) =>
    row.map((cell) => ({
      cost: cell.cost,
      category: cell.category,
      questionId: cell.questionId,
      correctAnswer: getAnswer(cell.questionId),
    }))
  ),
});

const getAnswer = (questionId: string | null): string | null =>
  questionId
    ? (questionsDb.find((q) => q.questionId === questionId)?.correctAnswer ?? null)
    : null;

export const packsHandlers = [
  http.get('/api/packs/cards', async ({ request }) => {
    await delay(100);
    return HttpResponse.json<PackCardDto[]>(paginate(request, packsCards));
  }),

  http.get('/api/packs/short', async ({ request }) => {
    await delay(100);
    const packs = packsCards.map(({ packId, name }) => ({
      packId,
      name,
    }));

    return HttpResponse.json<PackShort[]>(
      paginate(request, packs),
    );
  }),

  http.get('/api/packs/:id/edit', async ({ params }) => {
    await delay(100);
    const pack = packs.find((p) => p.packId === params.id);
    if (!pack) return new HttpResponse(null, { status: 404 });
    return HttpResponse.json<PackDto>(pack);
  }),

  http.patch('/api/packs/:id', async ({ params, request }) => {
    const id = params.id as string;
    const body = (await request.json()) as PackRequest;

    const existing = packs.find((p) => p.packId === id);

    const saved: PackDto = {
      packId: id,
      name: body.name,
      rounds: body.rounds.map(toRoundDto),
    };

    if (existing) Object.assign(existing, saved);
    else packs.push(saved);
    syncCard(saved);

    await delay(300);
    return new HttpResponse(null, { status: existing ? 204 : 201 });
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
];