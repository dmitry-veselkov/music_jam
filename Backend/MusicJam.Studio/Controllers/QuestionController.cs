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
    public async Task<IActionResult> GetQuestions([FromQuery] GetQuestionsQuery query, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var result = await questionService.GetQuestions(userId, query, ct);
        if (result.TryGetFault(out var error, out var questions))
            return NotFound(error);
        return Ok(new { questions });
    }

    [HttpPost]
    [ProducesResponseType(typeof(Question), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateQuestion([FromForm] CreateQuestionRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var result = await questionService.Create(userId, request, ct);
        if (result.TryGetFault(out var error, out var question))
            return BadRequest(error);
        return CreatedAtAction(nameof(GetQuestion), new { questionId = question.Id }, question);
    }

    [HttpGet("{questionId:guid}")]
    [ProducesResponseType(typeof(Question), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQuestion(Guid questionId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var result = await questionService.Get(userId, questionId, ct);
        if (result.TryGetFault(out var error, out var question))
            return NotFound(error);
        return Ok(question);
    }

    
    [HttpGet("{questionId:guid}/answer-track")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnswerTrack(Guid questionId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await questionService.GetAnswerTrack(userId, questionId, ct);
        if (result.TryGetFault(out var error, out var track))
            return NotFound(error);
        return Ok(track);
    }

    [HttpGet("{questionId:guid}/question-track")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQuestionTrack(Guid questionId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await questionService.GetQuestionTrack(userId, questionId, ct);
        if (result.TryGetFault(out var error, out var track))
            return NotFound(error);
        return Ok(track);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out userId);
    }
}