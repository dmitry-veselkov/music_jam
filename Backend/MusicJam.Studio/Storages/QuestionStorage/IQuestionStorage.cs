using MusicJam.Studio.Models.Packs;

namespace MusicJam.Studio.Storages;

public interface IQuestionStorage
{
    Task<List<Question>> GetAllUserQuestions(Guid id);

    Task<Question> SaveQuestion(Guid userId, Question track);
    
    Task<Question?> GetQuestion(Guid userId, Guid questionId);
    
    Task DeleteQuestion(Guid userId, Guid questionId);

}