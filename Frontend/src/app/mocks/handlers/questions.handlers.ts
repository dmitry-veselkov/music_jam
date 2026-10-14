import { delay, http, HttpResponse } from "msw";
import { questions } from "@/app/mocks/data/questions.ts";
import { paginate } from "@/app/mocks/paginate.ts";

export const questionsHandlers = [
  http.get('/api/questions', async ( { request } ) => {
    await delay(1000);
    return HttpResponse.json(paginate(request, questions));
  }),
  http.post('/api/questions', async ({ request }) => {
    const form = await request.formData();

    const created = {
      questionId: crypto.randomUUID(),
      requested: String(form.get('requested')),
      correctAnswer: String(form.get('correctAnswer')),
      questionTrack: form.get('questionTrack') as File | null,
      answerTrack: form.get('answerTrack') as File | null,
    };
    questions.push(created);

    await delay(300);
    return HttpResponse.json({ id: created.questionId }, { status: 201 });
  }),
  http.patch('/api/questions/:id', async ({ params, request }) => {
    const q = questions.find((q) => q.questionId === params.id);
    if (!q) return new HttpResponse(null, { status: 404 });
    const form = await request.formData();

    q.requested = String(form.get('requested'));
    q.correctAnswer = String(form.get('correctAnswer'));
    q.questionTrack = form.get('questionTrack') as File | null
    q.answerTrack = form.get('answerTrack') as File | null

    return HttpResponse.json(q);
  }),
  http.get('/api/questions/:id', ({ params }) => {
    const q = questions.find((q) => q.questionId === params.id);
    return HttpResponse.json(q);
  }),
  http.delete('/api/questions/:id', async ({ params }) => {
    const idx = questions.findIndex(q => q.questionId === params.id);
    if (idx === -1) return new HttpResponse(null, { status: 404 });

    questions.splice(idx, 1);
    return new HttpResponse(null, { status: 204 });
  })
]