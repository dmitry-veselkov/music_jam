using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrustructure.Data.Configurations;
using MusicJam.Studio.Infrustructure.Entities;
using MusicJam.Studio.Storages.QuestionStorage;
using DbQuestion = MusicJam.Studio.Infrustructure.Entities.Question;
using Question = MusicJam.Studio.Models.Packs.Question
    ;
namespace MusicJam.Studio.Storages.TrackStorage;

internal class QuestionStorage(AppDbContext db) : IQuestionStorage
{
    private static Question ToModel(DbQuestion e) => new()
    {
        Requested = e.Requested,
        CorrectAnswer = e.CorrectAnswer,
        AnswerTrack = TrackStorage.ToModel(e.AnswerTrack),
        QuestionTrack = TrackStorage.ToModel(e.QuestionTrack)
    };


    public async Task<List<Question>> TryGetAllUserQuestions(Guid id)
    {
        return db.Questions
            .Where(x => x.UserId == id)
            .Select(x => ToModel(x))
            .ToList();
    }

    public async Task<Question> SaveQuestion(Guid userId, Question question)
    {
        await db.Questions.AddAsync(new DbQuestion()
        {
            AnswerTrack = TrackStorage.ToDbModel(question.AnswerTrack),
            QuestionTrack =  TrackStorage.ToDbModel(question.QuestionTrack),
            CorrectAnswer = question.CorrectAnswer,
            Requested = question.Requested,
            UserId = userId
        });
        return question;
    }

    public async Task<Question?> TryGetQuestion(Guid userId, Guid questionId)
    {
        var question = await db.Questions
            .Where(x => x.UserId == userId && x.Id == questionId)
            .FirstOrDefaultAsync();
        return question is null ? null : ToModel(question);
    }

    public async Task DeleteQuestion(Guid userId, Guid questionId)
    {
        var question = await db.Questions
            .Where(x => x.UserId == userId && x.Id == questionId)
            .FirstOrDefaultAsync();
        if (question is null)
            return;
        db.Questions.Remove(question);
    }
}