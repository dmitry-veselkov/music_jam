using MusicJam.Studio.Requests.Questions;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.QuestionsServiceTests;

[Collection(TestCollection.Name)]
public class CreateQuestionTests(TestApplicationFixture fixture) : QuestionsServiceTestBase(fixture)
{
    private static readonly CreateQuestionRequest TestCreateQuestionRequest = new()
    {
        Requested = "Who is the artist?",
        CorrectAnswer = "John Lennon",
    };

    [Fact]
    public async Task Create_WhenRequestIsValid_CreatesQuestion()
    {
        var user = await CreateUserAsync();

        var result = await QuestionsService.Create(user.Id, TestCreateQuestionRequest);
        result.IsSuccess.ShouldBeTrue();

        var question = result.GetValueOrThrow();

        question.ShouldNotBeNull();
        question.Id.ShouldNotBe(Guid.Empty);
        question.Requested.ShouldBe(TestCreateQuestionRequest.Requested);
        question.CorrectAnswer.ShouldBe(TestCreateQuestionRequest.CorrectAnswer);
        question.QuestionTrack.ShouldNotBeNull();
        question.AnswerTrack.ShouldNotBeNull();
    }

    [Fact]
    public async Task Create_WhenRequestIsValid_SavesQuestionToDatabase()
    {
        var user = await CreateUserAsync();

        var result = await QuestionsService.Create(user.Id, TestCreateQuestionRequest);
        result.IsSuccess.ShouldBeTrue();

        var createdQuestion = result.GetValueOrThrow();

        var getResult = await QuestionsService.Get(user.Id, createdQuestion.Id);

        getResult.IsSuccess.ShouldBeTrue();

        var savedQuestion = getResult.GetValueOrThrow();
        savedQuestion.Id.ShouldBe(createdQuestion.Id);
        savedQuestion.Requested.ShouldBe(TestCreateQuestionRequest.Requested);
        savedQuestion.CorrectAnswer.ShouldBe(TestCreateQuestionRequest.CorrectAnswer);
    }
}
