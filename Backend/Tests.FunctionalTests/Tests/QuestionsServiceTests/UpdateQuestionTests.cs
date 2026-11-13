using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests.Questions;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.QuestionsServiceTests;

[Collection(TestCollection.Name)]
public class UpdateQuestionTests(TestApplicationFixture fixture) : QuestionsServiceTestBase(fixture)
{
    
    private static readonly UpdateQuestionRequest TestUpdateQuestionRequest = new()
    {
        Requested = "Who is the artist?",
        CorrectAnswer = "Mitski",
        AnswerTrack = CreateFile("answer.mp3"),
        QuestionTrack = CreateFile("question.mp3"),
    };
    
    [Fact]
    public async Task Update_WhenModePatchCopy_CreatesNewQuestionAndKeepsOriginal()
    {
        var user = await CreateUserAsync();

        var question = await CreateQuestionAsync(user.Id);
        var result = await QuestionsService.Update(
            user.Id,
            question.Id,
            PatchMode.PatchCopy,
            TestUpdateQuestionRequest);
        
       result.IsSuccess.ShouldBeTrue();
       var updated = result.GetValueOrThrow();
       updated.Id.ShouldNotBe(question.Id);
       updated.Requested.ShouldBe(TestUpdateQuestionRequest.Requested);
       updated.CorrectAnswer.ShouldBe(TestUpdateQuestionRequest.CorrectAnswer);
       var all = (await QuestionsService.GetQuestions(user.Id, new GetQuestionsRequest())).GetValueOrThrow();
       all.Length.ShouldBe(2);
       
       var original = (await QuestionsService.Get(user.Id, question.Id)).GetValueOrThrow();
       original.Requested.ShouldBe(question.Requested);
       original.CorrectAnswer.ShouldBe(question.CorrectAnswer);
       original.QuestionTrack.Id.ShouldBe(question.QuestionTrack.ObjectKey);
       original.AnswerTrack.Id.ShouldBe(question.AnswerTrack.ObjectKey);
    }

    [Fact]
    public async Task Update_WhenModeCascade_UpdatesSameQuestion()
    {
        var user = await CreateUserAsync();

        var question = await CreateQuestionAsync(user.Id);
        var result = await QuestionsService.Update(
            user.Id,
            question.Id,
            PatchMode.Cascade,
            TestUpdateQuestionRequest);
        
        result.IsSuccess.ShouldBeTrue();
        var updated = result.GetValueOrThrow();
        updated.Id.ShouldBe(question.Id);
        updated.Requested.ShouldBe(TestUpdateQuestionRequest.Requested);
        updated.CorrectAnswer.ShouldBe(TestUpdateQuestionRequest.CorrectAnswer);
        
        updated.QuestionTrack.Id.ShouldNotBe(question.QuestionTrack.ObjectKey);
        updated.AnswerTrack.Id.ShouldNotBe(question.AnswerTrack.ObjectKey);
        
        var all = (await QuestionsService.GetQuestions(user.Id, new GetQuestionsRequest())).GetValueOrThrow();
        all.Length.ShouldBe(1);
        
        var saved = (await QuestionsService.Get(user.Id, question.Id)).GetValueOrThrow();
        saved.Requested.ShouldBe(TestUpdateQuestionRequest.Requested);
        saved.CorrectAnswer.ShouldBe(TestUpdateQuestionRequest.CorrectAnswer);
    }
}