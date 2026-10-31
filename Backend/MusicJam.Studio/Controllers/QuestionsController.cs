using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Music;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Services.QuestionService;

namespace MusicJam.Studio.Controllers;

[ApiController]
[Route("api/questions")]
[Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
public class QuestionsController(IQuestionsService questionsService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Question[]>> GetQuestions([FromQuery] GetQuestionsRequest request)
    {
        var userId = User.GetUserId();

        var result = await questionsService.GetQuestions(userId, request);
        if (result.TryGetFault(out var error, out var questions))
        {
            return ErrorResponse(error);
        }

        return questions;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Question), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateQuestion([FromForm] CreateQuestionRequest request)
    {
        var userId = User.GetUserId();

        var result = await questionsService.Create(userId, request);
        if (result.TryGetFault(out var error, out var question))
        {
            return ErrorResponse(error);
        }

        return CreatedAtAction(
            nameof(GetQuestion),
            new { questionId = question.Id },
            question);
    }


    [HttpDelete("{questionId:guid}")]
    [ProducesResponseType(typeof(Question), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteQuestion(Guid questionId)
    {
        var userId = User.GetUserId();

        var result = await questionsService.Delete(userId, questionId);
        if (result.TryGetFault(out var error, out _))
        {
            return ErrorResponse(error);
        }

        return NoContent();
    }

    [HttpPatch("{questionId:guid}")]
    [ProducesResponseType(typeof(Question), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchQuestion(Guid questionId, [FromForm] CreateQuestionRequest request)
    {
        var userId = User.GetUserId();

        var result = await questionsService.Update(userId, questionId, PatchMode.PatchCopy, request);
        if (result.TryGetFault(out var error, out _))
        {
            return ErrorResponse(error);
        }

        return NoContent();
    }

    [HttpGet("{questionId:guid}")]
    [ProducesResponseType(typeof(Question), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQuestion(Guid questionId)
    {
        var userId = User.GetUserId();

        var result = await questionsService.Get(userId, questionId);
        if (result.TryGetFault(out var error, out var question))
        {
            return ErrorResponse(error);
        }

        return Ok(question);
    }


    [HttpGet("{questionId:guid}/answer-track")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnswerTrack(Guid questionId)
    {
        var userId = User.GetUserId();

        var result = await questionsService.GetTrack(userId, questionId, TrackType.Answer);
        if (result.TryGetFault(out var error, out var track))
        {
            return ErrorResponse(error);
        }

        return Ok(track);
    }

    [HttpGet("{questionId:guid}/question-track")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQuestionTrack(Guid questionId)
    {
        var userId = User.GetUserId();

        var result = await questionsService.GetTrack(userId, questionId, TrackType.Question);
        if (result.TryGetFault(out var error, out var track))
        {
            return ErrorResponse(error);
        }

        return Ok(track);
    }

    private ActionResult ErrorResponse(IQuestionsService.QuestionError error)
    {
        return error switch
        {
            IQuestionsService.QuestionError.NotFound => NotFound(),
            IQuestionsService.QuestionError.InvalidRequest => BadRequest(),
            IQuestionsService.QuestionError.InvalidTrackType => BadRequest(),
            _ => throw new ArgumentOutOfRangeException(error.ToString()),
        };
    }
}
