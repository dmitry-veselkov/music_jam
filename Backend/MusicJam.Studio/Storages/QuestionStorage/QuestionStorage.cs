using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Extensions;
using MusicJam.Studio.Infrustructure.Data;
using DbQuestion = MusicJam.Studio.Infrustructure.Entities.Question;
using Question = MusicJam.Studio.Models.Packs.Question;

namespace MusicJam.Studio.Storages.QuestionStorage;

public class QuestionStorage(AppDbContext db) : IQuestionStorage
{
    public async Task<List<Question>> GetAllUserQuestions(Guid id)
    {
        return await db.Questions
            .Include(q => q.QuestionTrack)
            .Include(q => q.AnswerTrack)
            .Where(x => x.UserId == id)
            .Select(x => x.ToModel())
            .ToListAsync();
    }

    public async Task<Question> SaveQuestion(Guid userId, Question question)
    {
        var entity = new DbQuestion
        {
            AnswerTrack = TrackStorage.TrackStorage.ToDbModel(question.AnswerTrack),
            QuestionTrack = TrackStorage.TrackStorage.ToDbModel(question.QuestionTrack),
            CorrectAnswer = question.CorrectAnswer,
            Requested = question.Requested,
            UserId = userId
        };

        await db.Questions.AddAsync(entity);
        await db.SaveChangesAsync();

        return entity.ToModel();
    }

    public async Task<Question?> GetQuestion(Guid userId, Guid questionId)
    {
        var question = await db.Questions
            .Include(q => q.QuestionTrack)
            .Include(q => q.AnswerTrack)
            .Where(x => x.UserId == userId && x.Id == questionId)
            .FirstOrDefaultAsync();

        return question?.ToModel();
    }

    public async Task DeleteQuestion(Guid userId, Guid questionId)
    {
        var question = await db.Questions
            .Where(x => x.UserId == userId && x.Id == questionId)
            .FirstOrDefaultAsync();

        if (question is null)
        {
            return;
        }

        db.Questions.Remove(question);

        await db.SaveChangesAsync();
    }
}
