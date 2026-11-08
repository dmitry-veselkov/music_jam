using MusicJam.Studio.Models.Packs;

namespace MusicJam.Studio.Storages.QuestionStorage;

public interface IQuestionStorage
{
    Task<List<Question>> GetAllUserQuestions(Guid userId, int? take = null, int skip = 0);
    
    Task<bool> HasUsages(Guid questionId);

    Task<Question> SaveQuestion(Guid userId, Question track);
    
    Task<Question?> GetQuestion(Guid userId, Guid questionId);
    
    Task<Question?> UpdateQuestion(Guid userId, Question question);

    Task DeleteQuestion(Guid userId, Guid questionId);

    Task<Dictionary<Guid, Question>> GetQuestions(Guid userId, IReadOnlyCollection<Guid> ids);
}