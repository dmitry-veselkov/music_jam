using MusicJam.Studio.Services.QuestionsService;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.QuestionsServiceTests;

[Collection(TestCollection.Name)]
public class DeleteQuestionTests(TestApplicationFixture fixture) : QuestionsServiceTestBase(fixture)
{
    [Fact]
    public async Task Delete_WhenQuestionDoesNotExist_ReturnsNotFound()
    {
        var user = await CreateUserAsync();
        var questionId = Guid.NewGuid();

        var result = await QuestionsService.Delete(user.Id, questionId);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IQuestionsService.QuestionError.NotFound);
    }

    [Fact]
    public async Task Delete_WhenQuestionExists_ReturnsQuestionId()
    {
        var user = await CreateUserAsync();
        var question = await CreateQuestionAsync(user.Id);

        var result = await QuestionsService.Delete(user.Id, question.Id);

        result.IsSuccess.ShouldBeTrue();
        result.GetValueOrThrow().ShouldBe(question.Id);
    }

    [Fact]
    public async Task Delete_WhenQuestionExists_DeletesQuestionFromDatabase()
    {
        var user = await CreateUserAsync();
        var question = await CreateQuestionAsync(user.Id);

        var deleteResult = await QuestionsService.Delete(user.Id, question.Id);

        deleteResult.IsSuccess.ShouldBeTrue();

        var getResult = await QuestionsService.Get(user.Id, question.Id);

        getResult.IsSuccess.ShouldBeFalse();
        getResult.GetFaultOrThrow().ShouldBe(IQuestionsService.QuestionError.NotFound);
    }

    [Fact]
    public async Task Delete_WhenQuestionBelongsToAnotherUser_ReturnsNotFound()
    {
        var owner = await CreateUserAsync();
        var anotherUser = await CreateUserAsync();

        var question = await CreateQuestionAsync(owner.Id);

        var result = await QuestionsService.Delete(anotherUser.Id, question.Id);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IQuestionsService.QuestionError.NotFound);

        var ownerResult = await QuestionsService.Get(owner.Id, question.Id);

        ownerResult.IsSuccess.ShouldBeTrue();
        ownerResult.GetValueOrThrow().Id.ShouldBe(question.Id);
    }
}