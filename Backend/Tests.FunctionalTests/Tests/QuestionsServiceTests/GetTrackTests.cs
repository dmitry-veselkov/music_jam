using MusicJam.Studio.Models.Tracks;
using MusicJam.Studio.Requests.Questions;
using MusicJam.Studio.Services.QuestionsService;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.QuestionsServiceTests;

[Collection(TestCollection.Name)]
public class GetTrackTests(TestApplicationFixture fixture)
    : QuestionsServiceTestBase(fixture)
{
    [Theory]
    [InlineData(TrackType.Question)]
    [InlineData(TrackType.Answer)]
    public async Task GetTrack_WhenTrackTypeIsValid_ReturnsGetObjectResponse(TrackType trackType)
    {
        var user = await CreateUserAsync();

        var question = await CreateQuestionAsync(user.Id);

        var result = await QuestionsService.GetTrack(
            user.Id,
            question.Id,
            trackType,
            null,
            null);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task GetTrack_WhenQuestionDoesNotExist_ReturnsNotFound()
    {
        var user = await CreateUserAsync();

        var result = await QuestionsService.GetTrack(
            user.Id,
            Guid.NewGuid(),
            TrackType.Question,
            null,
            null);

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
            TrackType.Question,
            null,
            null);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IQuestionsService.QuestionError.NotFound);
    }

    [Fact]
    public async Task GetTrack_WhenTrackTypeIsInvalid_ReturnsInvalidTrackType()
    {
        var user = await CreateUserAsync();

        var question = await CreateQuestionAsync(user.Id);

        var result = await QuestionsService.GetTrack(
            user.Id,
            question.Id,
            (TrackType)(-1),
            null,
            null);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IQuestionsService.QuestionError.InvalidTrackType);
    }
}
