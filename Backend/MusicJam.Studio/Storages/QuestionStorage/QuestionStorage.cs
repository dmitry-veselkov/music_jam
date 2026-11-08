using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Extensions;
using MusicJam.Studio.Infrastructure.Data;
using DbQuestion = MusicJam.Studio.Infrastructure.Entities.Question;
using Question = MusicJam.Studio.Models.Packs.Question;

namespace MusicJam.Studio.Storages.QuestionStorage;

public class QuestionStorage(AppDbContext db) : IQuestionStorage
{
    public Task<List<Question>> GetAllUserQuestions(Guid userId, int? take = null, int skip = 0)
    {
        var query = db.Questions
            .Where(x => x.UserId == userId)
            .Include(q => q.QuestionTrack)
            .Include(q => q.AnswerTrack)
            .Skip(skip);
        if (take.HasValue)
            query = query.Take(take.Value);
        
        return query
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

    public async Task<Question?> UpdateQuestion(Guid userId, Question question)
    {
        var previousQuestion = await db.Questions
            .Include(q => q.QuestionTrack)
            .Include(q => q.AnswerTrack)
            .Where(x => x.UserId == userId && x.Id == question.Id)
            .FirstOrDefaultAsync();

        if (previousQuestion is null)
        {
            return null;
        }

        previousQuestion.Requested = question.Requested;
        previousQuestion.CorrectAnswer = question.CorrectAnswer;

        if (previousQuestion.QuestionTrack.ObjectKey != question.QuestionTrack.Id)
        {
            previousQuestion.QuestionTrack = TrackStorage.TrackStorage.ToDbModel(question.QuestionTrack);
        }

        if (previousQuestion.AnswerTrack.ObjectKey != question.AnswerTrack.Id)
        {
            previousQuestion.AnswerTrack = TrackStorage.TrackStorage.ToDbModel(question.AnswerTrack);
        }
        await db.SaveChangesAsync();
        return previousQuestion.ToModel();
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

    public async Task<bool> HasUsages(Guid questionId)
    {
        return await db.QuestionUsages
            .AnyAsync(x => x.QuestionId == questionId);
    }
    
    public async Task<Dictionary<Guid, Question>> GetQuestions(Guid userId, IReadOnlyCollection<Guid> ids)
    {
        return await db.Questions
            .Include(q => q.QuestionTrack)
            .Include(q => q.AnswerTrack)
            .Where(x => x.UserId == userId && ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.ToModel());
    }

}
