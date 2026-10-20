using MusicJam.Studio.Models.Packs;

namespace MusicJam.Studio.Storages.QuestionStorage;

public interface IQuestionStorage
{
    Task<List<Question>> TryGetAllUserQuestions(Guid id);

    Task<Question> SaveQuestion(Guid userId, Question track);
    
    Task<Question?> TryGetQuestion(Guid userId, Guid questionId);
    
    Task DeleteQuestion(Guid userId, Guid questionId);

}