using MusicJam.Studio.Extensions;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Tracks;
using MusicJam.Studio.Requests.Questions;
using MusicJam.Studio.Responses;
using MusicJam.Studio.Responses.Questions;
using MusicJam.Studio.Services.TracksService;
using MusicJam.Studio.Storages.QuestionStorage;
using Shared;

namespace MusicJam.Studio.Services.QuestionsService;

internal class QuestionsService(
    IQuestionStorage questionStorage,
    ITracksService tracksService)
    : IQuestionsService
{
    // todo Elizaveta-Vinogradova
    // перетащить skip и take внутрь запроса к БД, чтобы не грузить лишнее
    public async Task<Result<QuestionCardResponse[], IQuestionsService.QuestionError>> GetQuestions(
        Guid userId,
        GetQuestionsRequest request)
    {
        var questions = await questionStorage.GetAllUserQuestions(userId);
        return questions
            .Select(question => question.ToCardResponse())
            .Skip(request.Skip)
            .Take(request.Take)
            .ToArray();
    }

    public async Task<Result<Question, IQuestionsService.QuestionError>> Create(
        Guid userId,
        CreateQuestionRequest request)
    {
        var uploadQuestionTrackResult = await tracksService.UploadTrack(request.QuestionTrack);
        if (uploadQuestionTrackResult.TryGetFault(out _, out var questionTrack))
        {
            return IQuestionsService.QuestionError.InvalidTrack;
        }

        var uploadAnswerTrackResult = await tracksService.UploadTrack(request.AnswerTrack);
        if (uploadAnswerTrackResult.TryGetFault(out _, out var answerTrack))
        {
            return IQuestionsService.QuestionError.InvalidTrack;
        }

        var question = new Question
        {
            AnswerTrack = new Track { Id = answerTrack.Id },
            QuestionTrack = new Track { Id = questionTrack.Id },
            Requested = request.Requested,
            CorrectAnswer = request.CorrectAnswer,
        };

        return await questionStorage.SaveQuestion(userId, question);
    }

    public async Task<Result<Question, IQuestionsService.QuestionError>> Get(Guid userId, Guid questionId)
    {
        var question = await questionStorage.GetQuestion(userId, questionId);
        if (question is null)
        {
            return IQuestionsService.QuestionError.NotFound;
        }

        return question;
    }

    // todo Elizaveta-Vinogradova
    // дописать проверку, что удаляемый вопрос нигде не используется
    // если же использования есть - будем либо просто блокировать такую операцию,
    // либо будет хотя бы предупреждать пользователя, что у вас такие-то паки сломаются
    public async Task<Result<Guid, IQuestionsService.QuestionError>> Delete(Guid userId, Guid questionId)
    {
        var question = await questionStorage.GetQuestion(userId, questionId);
        if (question is null)
        {
            return IQuestionsService.QuestionError.NotFound;
        }

        await questionStorage.DeleteQuestion(userId, questionId);
        return questionId;
    }

    // todo Elizaveta-Vinogradova
    // заглушка - реализовать в рамках общей задачи по тех. долгу
    public Task<Result<Question, IQuestionsService.QuestionError>> Update(
        Guid userId,
        Guid questionId,
        PatchMode mode,
        CreateQuestionRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TrackResponse, IQuestionsService.QuestionError>> GetTrack(
        Guid userId,
        Guid questionId,
        TrackType trackType,
        long? start,
        long? end)
    {
        var question = await questionStorage.GetQuestion(userId, questionId);
        if (question is null)
        {
            return IQuestionsService.QuestionError.NotFound;
        }

        var trackId = trackType switch
        {
            TrackType.Question => question.QuestionTrack?.Id,
            TrackType.Answer => question.AnswerTrack?.Id,
            _ => null
        };

        if (trackId is null)
        {
            return IQuestionsService.QuestionError.InvalidTrackType;
        }

        var sizeResult = await tracksService.GetTrackSize(trackId.Value);
        if (sizeResult.TryGetFault(out _, out var fileSize))
        {
            return IQuestionsService.QuestionError.CannotLoadTrack;
        }

        var rangeStart = start ?? 0;
        var rangeEnd = end ?? fileSize - 1;

        if (IsValidRange(fileSize, rangeStart, rangeEnd))
        {
            return IQuestionsService.QuestionError.CannotLoadTrack;
        }

        rangeEnd = Math.Min(rangeEnd, fileSize - 1);

        var trackResult = await tracksService.GetTrackRange(trackId.Value, rangeStart, rangeEnd);
        if (trackResult.TryGetFault(out _, out var track))
        {
            return IQuestionsService.QuestionError.CannotLoadTrack;
        }

        return new TrackResponse
        {
            TrackId = trackId.Value,
            Stream = track.ResponseStream,
            ContentType = track.Headers.ContentType,
            ContentLength = rangeEnd - rangeStart + 1,
            TotalLength = fileSize,
            Start = rangeStart,
            End = rangeEnd,
        };
    }

    private static bool IsValidRange(long fileSize, long rangeStart, long rangeEnd)
    {
        return fileSize <= 0
               || rangeStart < 0
               || rangeStart >= fileSize
               || rangeEnd < rangeStart;
    }
}
