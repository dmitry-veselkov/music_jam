using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using Shared;

namespace MusicJam.Studio.Services.QuestionService;

public interface IQuestionService
{
    public enum QuestionError
    {
        NotFound,        
        InvalidRequest,
    }
    
    Task<Result<List<Question>, QuestionError>> GetQuestions(Guid userId, GetQuestionsQuery query);

    Task<Result<Question, QuestionError>> Create(Guid userId, CreateQuestionRequest request);

    Task<Result<Question, QuestionError>> Get(Guid userId, Guid questionId);

    Task<Result<Guid, QuestionError>> Delete(Guid userId, Guid questionId);

    Task<Result<Question, QuestionError>> Update(Guid userId, Guid questionId, PatchMode mode, UpdateQuestionRequest request);

    Task<Result<string, QuestionError>> GetQuestionTrack(Guid userId, Guid questionId);

    Task<Result<string, QuestionError>> GetAnswerTrack(Guid userId, Guid questionId);
}