using MusicJam.Music;
using MusicJam.Studio.Helpers;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Users;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Services.TokenService;
using MusicJam.Studio.Storages.QuestionStorage;
using MusicJam.Studio.Storages.TrackStorage;
using MusicJam.Studio.Storages.UsersStorage;
using Shared;

namespace MusicJam.Studio.Services.QuestionService;

internal class QuestionService(
    IQuestionStorage questionStorage,
    ITrackStorage trackStorage)
    : IQuestionService
{
    public async Task<Result<List<Question>, IQuestionService.QuestionError>> GetQuestions(Guid userId, GetQuestionsQuery query, CancellationToken ct)
    {
        var questions = await questionStorage.TryGetAllUserQuestions(userId);
        return questions
            .Skip(query.Skip)
            .Take(query.Take)
            .ToList();
    }

    public async Task<Result<Question, IQuestionService.QuestionError>> Create(Guid userId, CreateQuestionRequest request, CancellationToken ct)
    {
        var question = new Question()
        {
            AnswerTrack = new Track()
            {
                Link = request.AnswerTrackFile
            },
            QuestionTrack = new Track()
            {
                Link = request.QuestionTrackFile
            },
            Requested = request.Requested,
            CorrectAnswer = request.CorrectAnswer,
        };
        await questionStorage.SaveQuestion(userId, question);
        return question;
    }

    public async Task<Result<Question, IQuestionService.QuestionError>> Get(Guid userId, Guid questionId, CancellationToken ct)
    {
        var question = await questionStorage.TryGetQuestion(userId, questionId);
        if (question is null)
            return IQuestionService.QuestionError.NotFound;
        return question;
    }

    public async Task<Result<Guid, IQuestionService.QuestionError>> Delete(Guid userId, Guid questionId, CancellationToken ct)
    {
        var question = await questionStorage.TryGetQuestion(userId, questionId);
        if (question is null)
            return IQuestionService.QuestionError.NotFound;
        await questionStorage.DeleteQuestion(userId, questionId);
        return questionId;
    }

    public Task<Result<Question, IQuestionService.QuestionError>> Update(Guid userId, Guid questionId, PatchMode mode, UpdateQuestionRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<string, IQuestionService.QuestionError>> GetQuestionTrack(Guid userId, Guid questionId, CancellationToken ct)
    {
        var question = await questionStorage.TryGetQuestion(userId, questionId);
        if (question is null)
            return IQuestionService.QuestionError.NotFound;
        return question.QuestionTrack.Link;
    }

    public async Task<Result<string, IQuestionService.QuestionError>> GetAnswerTrack(Guid userId, Guid questionId, CancellationToken ct)
    {
        var question = await questionStorage.TryGetQuestion(userId, questionId);
        if (question is null)
            return IQuestionService.QuestionError.NotFound;
        return question.AnswerTrack.Link;
    }
}