using Microsoft.AspNetCore.Http;
using MusicJam.Studio.Infrastructure.Entities;
using MusicJam.Studio.Services.QuestionsService;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.QuestionsServiceTests;

public abstract class QuestionsServiceTestBase(TestApplicationFixture fixture) : FunctionalTestBase(fixture)
{
    protected IQuestionsService QuestionsService => GetService<IQuestionsService>();

    protected async Task<User> CreateUserAsync(IEnumerable<Question>? questions = null)
    {
        var user = new User
        {
            Username = "John Doe",
            Email = "example.com",
            PasswordHash = "hash",
            Questions = questions?.ToList() ?? []
        };

        await SeedAsync(user);

        return user;
    }

    protected async Task<Question> CreateQuestionAsync(
        Guid userId,
        string requested = "Test question",
        string correctAnswer = "Test answer")
    {
        var question = await GetQuestion(userId, requested, correctAnswer);
        await SeedAsync(question);

        return question;
    }

    protected async Task<List<Question>> CreateQuestionsAsync(Guid userId, int count)
    {
        var questionTasks = Enumerable.Range(1, count)
            .Select(i => GetQuestion(
                userId,
                $"Requested {i}",
                $"Correct answer {i}"));

        var questions = await Task.WhenAll(questionTasks);

        foreach (var question in questions)
        {
            await SeedAsync(question);
        }

        return questions.ToList();
    }

    private async Task<Question> GetQuestion(Guid userId, string requested, string correctAnswer)
    {
        var questionTrackKey = await UploadObjectAsync(CreateFile("question.mp3"));
        var answerTrackKey = await UploadObjectAsync(CreateFile("answer.mp3"));

        var question = new Question
        {
            UserId = userId,
            Requested = requested,
            CorrectAnswer = correctAnswer,
            AnswerTrack = new Track { ObjectKey = answerTrackKey },
            QuestionTrack = new Track { ObjectKey = questionTrackKey },
        };

        return question;
    }

    protected static IFormFile CreateFile(string fileName = "audio.mp3", long fileSize = 20 * 1024)
    {
        var bytes = new byte[fileSize];
        Random.Shared.NextBytes(bytes);

        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "audio/mpeg"
        };
    }
}
