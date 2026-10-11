import { delay, http, HttpResponse } from "msw";
import { questions } from "@/app/mocks/data/questions.ts";

const fileName = (value: FormDataEntryValue | null) =>
  value instanceof File ? value.name : '';

export const questionsHandlers = [
  http.get('/api/questions', async () => {
    await delay(1000);
    return HttpResponse.json(questions);
  }),
  http.post('/api/questions', async ({ request }) => {
    const form = await request.formData();

    const created = {
      questionId: crypto.randomUUID(),
      requested: String(form.get('requested')),
      correctAnswer: String(form.get('correctAnswer')),
      questionTrack: fileName(form.get('questionTrack')),
      answerTrack: fileName(form.get('answerTrack')),
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

    const qName = fileName(form.get('questionTrack'));
    const aName = fileName(form.get('answerTrack'));
    if (qName) q.questionTrack = qName;
    if (aName) q.answerTrack = aName;
    console.log(q)
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