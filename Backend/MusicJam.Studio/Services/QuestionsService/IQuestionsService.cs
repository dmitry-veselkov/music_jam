using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Tracks;
using MusicJam.Studio.Requests.Questions;
using MusicJam.Studio.Responses;
using MusicJam.Studio.Responses.Questions;
using Shared;

namespace MusicJam.Studio.Services.QuestionsService;

public interface IQuestionsService
{
    public enum QuestionError
    {
        NotFound,
        InvalidTrack,
        InvalidTrackType,
        CannotLoadTrack,
    }

    Task<Result<QuestionCardResponse[], QuestionError>> GetQuestions(Guid userId, GetQuestionsRequest request);

    Task<Result<Question, QuestionError>> Create(Guid userId, CreateQuestionRequest request);

    Task<Result<Question, QuestionError>> Get(Guid userId, Guid questionId);

    Task<Result<Guid, QuestionError>> Delete(Guid userId, Guid questionId);

    Task<Result<Question, QuestionError>> Update(Guid userId, Guid questionId, PatchMode mode,
        CreateQuestionRequest request);

    Task<Result<TrackResponse, QuestionError>> GetTrack(
        Guid userId,
        Guid questionId,
        TrackType trackType,
        long? start,
        long? end);
}
