using Microsoft.EntityFrameworkCore;
using MusicJam.Music;
using MusicJam.Studio.Infrustructure.Data.Configurations;
using DbQuestion = MusicJam.Studio.Infrustructure.Entities.Question;
using Question = MusicJam.Studio.Models.Packs.Question;

namespace MusicJam.Studio.Storages;

public class QuestionStorage(AppDbContext db) : IQuestionStorage
{
    public static Question ToModel(DbQuestion e) => new()
    {
        Id = e.Id,
        Requested = e.Requested,
        CorrectAnswer = e.CorrectAnswer,
        AnswerTrack = new Track()
        {
            Link = e.AnswerTrack.Link
        },
        QuestionTrack = new Track()
        {
            Link = e.QuestionTrack.Link
        },
    };


    public async Task<List<Question>> GetAllUserQuestions(Guid id)
    {
        return db.Questions
            .Include(q => q.QuestionTrack)
            .Include(q => q.AnswerTrack)
            .Where(x => x.UserId == id)
            .Select(x => ToModel(x))
            .ToList();
    }

    public async Task<Question> SaveQuestion(Guid userId, Question question)
    {
        var entity = new DbQuestion()
        {
            AnswerTrack = TrackStorage.ToDbModel(question.AnswerTrack),
            QuestionTrack =  TrackStorage.ToDbModel(question.QuestionTrack),
            CorrectAnswer = question.CorrectAnswer,
            Requested = question.Requested,
            UserId = userId
        };
        await db.Questions.AddAsync(entity);
        await db.SaveChangesAsync();
        return ToModel(entity);
    }

    public async Task<Question?> GetQuestion(Guid userId, Guid questionId)
    {
        var question = await db.Questions
            .Include(q => q.QuestionTrack)
            .Include(q => q.AnswerTrack)
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
        await db.SaveChangesAsync();
    }
}