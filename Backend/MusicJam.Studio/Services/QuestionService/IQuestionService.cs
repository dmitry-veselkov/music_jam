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
    
    Task<Result<List<Question>, QuestionError>> GetQuestions(Guid userId, GetQuestionsQuery query, CancellationToken ct);

    Task<Result<Question, QuestionError>> Create(Guid userId, CreateQuestionRequest request, CancellationToken ct);

    Task<Result<Question, QuestionError>> Get(Guid userId, Guid questionId, CancellationToken ct);

    Task<Result<Guid, QuestionError>> Delete(Guid userId, Guid questionId, CancellationToken ct);

    Task<Result<Question, QuestionError>> Update(Guid userId, Guid questionId, PatchMode mode, UpdateQuestionRequest request, CancellationToken ct);

    Task<Result<string, QuestionError>> GetQuestionTrack(Guid userId, Guid questionId, CancellationToken ct);

    Task<Result<string, QuestionError>> GetAnswerTrack(Guid userId, Guid questionId, CancellationToken ct);
}