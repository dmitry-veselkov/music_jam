using MusicJam.Studio.Requests.Questions;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.QuestionsServiceTests;

[Collection(TestCollection.Name)]
public class GetQuestionsTests(TestApplicationFixture fixture) : QuestionsServiceTestBase(fixture)
{
    [Fact]
    public async Task GetQuestions_WhenUserHasNoQuestions_ReturnsEmptyArray()
    {
        var user = await CreateUserAsync();

        var result = await QuestionsService.GetQuestions(user.Id, new GetQuestionsRequest());

        result.IsSuccess.ShouldBeTrue();
        result.GetValueOrThrow().ShouldBeEmpty();
    }

    [Fact]
    public async Task GetQuestions_WhenUserHasQuestions_ReturnsAllQuestions()
    {
        var user = await CreateUserAsync();
        var questions = await CreateQuestionsAsync(user.Id, 3);

        var result = await QuestionsService.GetQuestions(user.Id, new GetQuestionsRequest());
        result.IsSuccess.ShouldBeTrue();

        var foundQuestions = result.GetValueOrThrow();

        foundQuestions.Length.ShouldBe(3);
        foundQuestions
            .Select(x => x.QuestionId)
            .ShouldBeEquivalentTo(questions.Select(x => x.Id));
    }

    [Fact]
    public async Task GetQuestions_WhenTakeIsLessThanQuestionCount_ReturnsLimitedQuestions()
    {
        var user = await CreateUserAsync();
        var questions = await CreateQuestionsAsync(user.Id, 3);

        var result = await QuestionsService.GetQuestions(user.Id, new GetQuestionsRequest { Take = 2 });
        result.IsSuccess.ShouldBeTrue();

        var foundQuestions = result.GetValueOrThrow();

        foundQuestions.Length.ShouldBe(2);
        foundQuestions
            .Select(x => x.QuestionId)
            .ShouldBeEquivalentTo(questions
                .Take(2)
                .Select(x => x.Id));
    }

    [Fact]
    public async Task GetQuestions_WhenSkipIsSpecified_SkipsQuestions()
    {
        var user = await CreateUserAsync();
        var questions = await CreateQuestionsAsync(user.Id, 5);

        var result = await QuestionsService.GetQuestions(user.Id, new GetQuestionsRequest { Skip = 2 });
        result.IsSuccess.ShouldBeTrue();

        var foundQuestions = result.GetValueOrThrow();
        foundQuestions.Length.ShouldBe(3);
        foundQuestions
            .Select(x => x.QuestionId)
            .ShouldBeEquivalentTo(questions
                .Skip(2)
                .Select(x => x.Id));
    }
}
