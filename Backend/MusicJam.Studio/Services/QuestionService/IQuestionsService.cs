using MusicJam.Music;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests.Questions;
using Shared;

namespace MusicJam.Studio.Services.QuestionService;

public interface IQuestionsService
{
    public enum QuestionError
    {
        NotFound,
        InvalidRequest,
        InvalidTrackType,
    }

    Task<Result<Question[], QuestionError>> GetQuestions(Guid userId, GetQuestionsRequest request);

    Task<Result<Question, QuestionError>> Create(Guid userId, CreateQuestionRequest request);

    Task<Result<Question, QuestionError>> Get(Guid userId, Guid questionId);

    Task<Result<Guid, QuestionError>> Delete(Guid userId, Guid questionId);

    Task<Result<Question, QuestionError>> Update(Guid userId, Guid questionId, PatchMode mode,
        CreateQuestionRequest request);

    Task<Result<string, QuestionError>> GetTrack(Guid userId, Guid questionId, TrackType trackType);
}
