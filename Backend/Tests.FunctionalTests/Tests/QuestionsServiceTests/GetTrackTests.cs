using MusicJam.Music;
using MusicJam.Studio.Requests.Questions;
using MusicJam.Studio.Services.QuestionService;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.QuestionsServiceTests;

[Collection(TestCollection.Name)]
public class GetTrackTests(TestApplicationFixture fixture)
    : QuestionsServiceTestBase(fixture)
{
    [Theory]
    [InlineData(TrackType.Question, "question")]
    [InlineData(TrackType.Answer, "answer")]
    public async Task GetTrack_WhenTrackTypeIsValid_ReturnsTrackLink(TrackType trackType, string expectedLink)
    {
        var user = await CreateUserAsync();

        var question = await QuestionsService.Create(
            user.Id,
            new CreateQuestionRequest
            {
                Requested = "Test question",
                CorrectAnswer = "Test answer"
            });

        question.IsSuccess.ShouldBeTrue();

        var result = await QuestionsService.GetTrack(
            user.Id,
            question.GetValueOrThrow().Id,
            trackType);

        result.IsSuccess.ShouldBeTrue();
        result.GetValueOrThrow().ShouldBe(expectedLink);
    }

    [Fact]
    public async Task GetTrack_WhenQuestionDoesNotExist_ReturnsNotFound()
    {
        var user = await CreateUserAsync();

        var result = await QuestionsService.GetTrack(
            user.Id,
            Guid.NewGuid(),
            TrackType.Question);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IQuestionsService.QuestionError.NotFound);
    }

    [Fact]
    public async Task GetTrack_WhenQuestionBelongsToAnotherUser_ReturnsNotFound()
    {
        var owner = await CreateUserAsync();
        var anotherUser = await CreateUserAsync();

        var question = await CreateQuestionAsync(owner.Id);

        var result = await QuestionsService.GetTrack(
            anotherUser.Id,
            question.Id,
            TrackType.Question);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IQuestionsService.QuestionError.NotFound);
    }

    [Fact]
    public async Task GetTrack_WhenTrackTypeIsInvalid_ReturnsInvalidTrackType()
    {
        var user = await CreateUserAsync();

        var question = await QuestionsService.Create(
            user.Id,
            new CreateQuestionRequest
            {
                Requested = "Test question",
                CorrectAnswer = "Test answer"
            });

        question.IsSuccess.ShouldBeTrue();

        var result = await QuestionsService.GetTrack(
            user.Id,
            question.GetValueOrThrow().Id,
            (TrackType)(-1));

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IQuestionsService.QuestionError.InvalidTrackType);
    }
}