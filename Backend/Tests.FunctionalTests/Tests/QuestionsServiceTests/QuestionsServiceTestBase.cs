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
    
    protected static IFormFile CreateTestFile(string fileName = "track.mp3")
    {
        var bytes = "fake audio content"u8.ToArray();

        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "audio/mpeg"
        };
    }

    protected async Task<Question> CreateQuestionAsync(
        Guid userId,
        string requested = "Test question",
        string correctAnswer = "Test answer")
    {
        var question = new Question
        {
            UserId = userId,
            Requested = requested,
            CorrectAnswer = correctAnswer,
            AnswerTrack = new Track { ObjectKey = Guid.NewGuid() },
            QuestionTrack = new Track { ObjectKey = Guid.NewGuid() },
        };

        await SeedAsync(question);

        return question;
    }

    protected async Task<List<Question>> CreateQuestionsAsync(Guid userId, int count)
    {
        var questions = Enumerable.Range(1, count)
            .Select(i => new Question
            {
                UserId = userId,
                Requested = $"Requested {i}",
                CorrectAnswer = $"Correct answer {i}",
                AnswerTrack = new Track { ObjectKey = Guid.NewGuid() },
                QuestionTrack = new Track { ObjectKey = Guid.NewGuid() },
            })
            .ToList();

        foreach (var question in questions)
        {
            await SeedAsync(question);
        }

        return questions;
    }
}
