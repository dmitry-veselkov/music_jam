import { delay, http, HttpResponse } from 'msw';
import { paginate } from '@/app/mocks/paginate.ts';
import { questionsDb, toCard } from '@/app/mocks/data/questionsDb.ts';

const isFile = (v: FormDataEntryValue | null): v is File => v instanceof File;

export const questionsHandlers = [
  http.get('/api/questions', async ({ request }) => {
    await delay(1000);
    return HttpResponse.json(paginate(request, questionsDb.map(toCard)));
  }),

  http.post('/api/questions', async ({ request }) => {
    const form = await request.formData();
    const questionTrack = form.get('questionTrack');
    const answerTrack = form.get('answerTrack');

    if (!isFile(questionTrack) || !isFile(answerTrack)) {
      return new HttpResponse(null, { status: 400 });
    }

    const created = {
      questionId: crypto.randomUUID(),
      requested: String(form.get('requested')),
      correctAnswer: String(form.get('correctAnswer')),
      questionTrack,
      answerTrack,
    };
    questionsDb.push(created);

    await delay(300);
    return HttpResponse.json({ id: created.questionId }, { status: 201 });
  }),

  http.patch('/api/questions/:id', async ({ params, request }) => {
    const q = questionsDb.find((q) => q.questionId === params.id);
    if (!q) return new HttpResponse(null, { status: 404 });

    const form = await request.formData();
    q.requested = String(form.get('requested') ?? q.requested);
    q.correctAnswer = String(form.get('correctAnswer') ?? q.correctAnswer);

    const questionTrack = form.get('questionTrack');
    const answerTrack = form.get('answerTrack');
    if (isFile(questionTrack)) q.questionTrack = questionTrack;
    if (isFile(answerTrack)) q.answerTrack = answerTrack;

    return HttpResponse.json(toCard(q));
  }),

  http.get('/api/questions/:id', ({ params }) => {
    const q = questionsDb.find((q) => q.questionId === params.id);
    if (!q) return new HttpResponse(null, { status: 404 });

    return HttpResponse.json({
      ...toCard(q),
      questionTrackUrl: `/api/questions/${q.questionId}/question-track`,
      answerTrackUrl: `/api/questions/${q.questionId}/answer-track`,
    });
  }),

  http.get('/api/questions/:id/:track', async ({ params }) => {
    const q = questionsDb.find((q) => q.questionId === params.id);
    if (!q) return new HttpResponse(null, { status: 404 });

    const file = params.track === 'question-track' ? q.questionTrack : q.answerTrack;
    return new HttpResponse(await file.arrayBuffer(), {
      headers: { 'Content-Type': file.type },
    });
  }),

  http.delete('/api/questions/:id', async ({ params }) => {
    const idx = questionsDb.findIndex((q) => q.questionId === params.id);
    if (idx === -1) return new HttpResponse(null, { status: 404 });

    questionsDb.splice(idx, 1);
    return new HttpResponse(null, { status: 204 });
  }),
];