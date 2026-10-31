using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Services.QuestionService;

namespace MusicJam.Studio.Controllers;

[ApiController]
[Route("api/questions")]
[Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
public class QuestionController(IQuestionService questionService) : ControllerBase
{
   
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQuestions([FromQuery] GetQuestionsRequest request)
    {
        var userId = User.GetUserId();
        var result = await questionService.GetQuestions(userId, request);
        if (result.TryGetFault(out var error, out var questions))
            return ErrorResponse(error);
        return Ok(questions);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Question), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateQuestion([FromForm] CreateQuestionRequest request)
    {
        var userId = User.GetUserId();
        var result = await questionService.Create(userId, request);
        if (result.TryGetFault(out var error, out var question))
            return ErrorResponse(error);
        return CreatedAtAction(nameof(GetQuestion), new { questionId = question.Id }, question);
    }


    [HttpDelete("{questionId:guid}")]
    [ProducesResponseType(typeof(Question), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteQuestion(Guid questionId)
    {
        var userId = User.GetUserId();
        var result = await questionService.Delete(userId, questionId);
        if (result.TryGetFault(out var error, out var question))
            return ErrorResponse(error);
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
        var result = await questionService.Update(userId, questionId, PatchMode.PatchCopy, request);
        if (result.TryGetFault(out var error, out var question))
            return ErrorResponse(error);
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
        var result = await questionService.Get(userId, questionId);
        if (result.TryGetFault(out var error, out var question))
            return ErrorResponse(error);
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
        var result = await questionService.GetAnswerTrack(userId, questionId);
        if (result.TryGetFault(out var error, out var track))
            return ErrorResponse(error);
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
        var result = await questionService.GetQuestionTrack(userId, questionId);
        if (result.TryGetFault(out var error, out var track))
            return ErrorResponse(error);
        return Ok(track);
    }
}