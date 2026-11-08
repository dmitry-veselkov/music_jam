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
    public async Task<Result<QuestionCardResponse[], IQuestionsService.QuestionError>> GetQuestions(
        Guid userId,
        GetQuestionsRequest request)
    {
        var questions = await questionStorage.GetAllUserQuestions(userId, request.Take, request.Skip);
        return questions
            .Select(question => question.ToCardResponse())
            .ToArray();
    }

    public async Task<Result<Question, IQuestionsService.QuestionError>> Create(
        Guid userId,
        CreateQuestionRequest request)
    {
        var uploadQuestionTrackResult = await tracksService.UploadTrack(request.QuestionTrack);
        if (uploadQuestionTrackResult.TryGetFault(out _, out var questionTrack))
        {
            return IQuestionsService.QuestionError.CannotLoadTrack;
        }

        var uploadAnswerTrackResult = await tracksService.UploadTrack(request.AnswerTrack);
        if (uploadAnswerTrackResult.TryGetFault(out _, out var answerTrack))
        {
            return IQuestionsService.QuestionError.CannotLoadTrack;
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
    
    public async Task<Result<Guid, IQuestionsService.QuestionError>> Delete(Guid userId, Guid questionId)
    {
        var question = await questionStorage.GetQuestion(userId, questionId);
        if (question is null)
        {
            return IQuestionsService.QuestionError.NotFound;
        }

        var hasUsages = await questionStorage.HasUsages(questionId);
        if (hasUsages)
            return IQuestionsService.QuestionError.InUse;
        
        await questionStorage.DeleteQuestion(userId, questionId);
        return questionId;
    }
    
    public async Task<Result<Question, IQuestionsService.QuestionError>> Update(
        Guid userId,
        Guid questionId,
        PatchMode mode,
        UpdateQuestionRequest request)
    {
        var previousQuestion = await questionStorage.GetQuestion(userId, questionId);
        if (previousQuestion is null)
        {
            return IQuestionsService.QuestionError.NotFound;
        }
        
        var questionTrackResult = await ResolveTrack(request.QuestionTrack, previousQuestion.QuestionTrack);
        if (questionTrackResult.TryGetFault(out var questionTrackError, out var questionTrack))
        {
            return questionTrackError;
        }

        var answerTrackResult = await ResolveTrack(request.AnswerTrack, previousQuestion.AnswerTrack);
        if (answerTrackResult.TryGetFault(out var answerTrackError, out var answerTrack))
        {
            return answerTrackError;
        }

        var question = new Question
        {
            Id = questionId,
            Requested = request.Requested ?? previousQuestion.Requested,
            CorrectAnswer = request.CorrectAnswer ?? previousQuestion.CorrectAnswer,
            QuestionTrack = questionTrack,
            AnswerTrack = answerTrack,
        };

        switch (mode)
        {
            case PatchMode.Cascade:
                var updated = await questionStorage.UpdateQuestion(userId, question);
                if (updated is null)
                {
                    return IQuestionsService.QuestionError.NotFound;
                }
                return updated;
            case PatchMode.PatchCopy:
                return await questionStorage.SaveQuestion(userId, question);
            default:
                throw new ArgumentOutOfRangeException(mode.ToString());
        }
    }
    
    private async Task<Result<Track, IQuestionsService.QuestionError>> ResolveTrack(
        IFormFile? newFile,
        Track previousTrack)
    {
        if (newFile is null)
        {
            return new Track { Id = previousTrack.Id };
        }

        var uploadResult = await tracksService.UploadTrack(newFile);
        if (uploadResult.TryGetFault(out _, out var uploaded))
        {
            return IQuestionsService.QuestionError.InvalidTrack;
        }
        return new Track { Id = uploaded.Id };
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
