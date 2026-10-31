using MusicJam.Music;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Storages;
using Shared;

namespace MusicJam.Studio.Services.QuestionService;

internal class QuestionService(
    IQuestionStorage questionStorage,
    ITrackStorage trackStorage)
    : IQuestionService
{
    public async Task<Result<List<Question>, IQuestionService.QuestionError>> GetQuestions(Guid userId, GetQuestionsRequest request)
    {
        var questions = await questionStorage.GetAllUserQuestions(userId);
        return questions
            .Skip(request.Skip)
            .Take(request.Take)
            .ToList();
    }

    public async Task<Result<Question, IQuestionService.QuestionError>> Create(Guid userId, CreateQuestionRequest request)
    {
        //в s3
        var question = new Question()
        {
            AnswerTrack = new Track()
            {
                Link = "answer"
            },
            QuestionTrack = new Track()
            {
                Link = "question"
            },
            Requested = request.Requested,
            CorrectAnswer = request.CorrectAnswer,
        };
        return await questionStorage.SaveQuestion(userId, question);
    }

    public async Task<Result<Question, IQuestionService.QuestionError>> Get(Guid userId, Guid questionId)
    {
        var question = await questionStorage.GetQuestion(userId, questionId);
        if (question is null)
            return IQuestionService.QuestionError.NotFound;
        return question;
    }

    public async Task<Result<Guid, IQuestionService.QuestionError>> Delete(Guid userId, Guid questionId)
    {
        var question = await questionStorage.GetQuestion(userId, questionId);
        if (question is null)
            return IQuestionService.QuestionError.NotFound;
        await questionStorage.DeleteQuestion(userId, questionId);
        return questionId;
    }

    public Task<Result<Question, IQuestionService.QuestionError>> Update(Guid userId, Guid questionId, PatchMode mode, CreateQuestionRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<string, IQuestionService.QuestionError>> GetQuestionTrack(Guid userId, Guid questionId)
    {
        var question = await questionStorage.GetQuestion(userId, questionId);
        if (question is null)
            return IQuestionService.QuestionError.NotFound;
        return question.QuestionTrack.Link;
    }

    public async Task<Result<string, IQuestionService.QuestionError>> GetAnswerTrack(Guid userId, Guid questionId)
    {
        var question = await questionStorage.GetQuestion(userId, questionId);
        if (question is null)
            return IQuestionService.QuestionError.NotFound;
        return question.AnswerTrack.Link;
    }
}