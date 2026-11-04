using MusicJam.Studio.Services.QuestionsService;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.QuestionsServiceTests;

[Collection(TestCollection.Name)]
public class GetQuestionTests(TestApplicationFixture fixture) : QuestionsServiceTestBase(fixture)
{
    [Fact]
    public async Task Get_WhenQuestionDoesNotExist_ReturnsNotFound()
    {
        var user = await CreateUserAsync();
        var questionId = Guid.NewGuid();

        var result = await QuestionsService.Get(user.Id, questionId);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IQuestionsService.QuestionError.NotFound);
    }

    [Fact]
    public async Task Get_WhenQuestionExists_ReturnsQuestion()
    {
        var user = await CreateUserAsync();
        var questionDb = await CreateQuestionAsync(
            user.Id,
            "Who is the artist?",
            "John Lennon");

        var result = await QuestionsService.Get(user.Id, questionDb.Id);

        result.IsSuccess.ShouldBeTrue();

        var question = result.GetValueOrThrow();

        question.ShouldNotBeNull();
        question.Id.ShouldBe(questionDb.Id);
        question.Requested.ShouldBe(questionDb.Requested);
        question.CorrectAnswer.ShouldBe(questionDb.CorrectAnswer);
    }

    [Fact]
    public async Task Get_WhenQuestionBelongsToAnotherUser_ReturnsNotFound()
    {
        var owner = await CreateUserAsync();
        var anotherUser = await CreateUserAsync();

        var question = await CreateQuestionAsync(owner.Id);

        var result = await QuestionsService.Get(anotherUser.Id, question.Id);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IQuestionsService.QuestionError.NotFound);
    }
}
