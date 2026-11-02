using MusicJam.Music;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests.Questions;
using MusicJam.Studio.Storages;
using Shared;

namespace MusicJam.Studio.Services.QuestionService;

internal class QuestionsService(
    IQuestionStorage questionStorage,
    ITrackStorage trackStorage)
    : IQuestionsService
{
    // todo Elizaveta-Vinogradova
    // перетащить skip и take внутрь запроса к БД, чтобы не грузить лишнее
    public async Task<Result<Question[], IQuestionsService.QuestionError>> GetQuestions(
        Guid userId,
        GetQuestionsRequest request)
    {
        var questions = await questionStorage.GetAllUserQuestions(userId);
        return questions
            .Skip(request.Skip)
            .Take(request.Take)
            .ToArray();
    }

    // todo Elizaveta-Vinogradova
    // заглушка - решить в рамках задачи https://buildin.ai/fiit-urfu/bee9347e-e145-4921-8451-3ef40771a16a
    public async Task<Result<Question, IQuestionsService.QuestionError>> Create(
        Guid userId,
        CreateQuestionRequest request)
    {
        var question = new Question
        {
            AnswerTrack = new Track
            {
                Link = "answer"
            },
            QuestionTrack = new Track
            {
                Link = "question"
            },
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

    public async Task<Result<string, IQuestionsService.QuestionError>> GetTrack(
        Guid userId,
        Guid questionId,
        TrackType trackType)
    {
        var question = await questionStorage.GetQuestion(userId, questionId);
        if (question is null)
        {
            return IQuestionsService.QuestionError.NotFound;
        }

        return trackType switch
        {
            TrackType.Question => question.QuestionTrack.Link,
            TrackType.Answer => question.AnswerTrack.Link,
            _ => IQuestionsService.QuestionError.InvalidTrackType
        };
    }
}
