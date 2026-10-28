using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Responses;
using MusicJam.Studio.Services.PackService;

namespace MusicJam.Studio.Controllers;

[ApiController]
[Route("api/packs")]
[Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
public class PackController(IPackService packService, IMapper mapper) : ControllerBase
{
   
    [HttpGet("short")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetShortPack([FromQuery] GetPackRequest query)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var result = await packService.GetShort(userId, query);
        return Ok(result.Select(x => new ShortPackResponse { PackId = x.Item1, Name = x.Item2 }));
    }
    
    [HttpGet("cards")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    public async Task<IActionResult> GetCardPacks([FromQuery] GetPackRequest query)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var result = await packService.GetPacks(userId, query);
        var packDto = result
            .Select(mapper.Map<GetPackResponse>)
            .ToList();
        return Ok(packDto);
    }
    

    [HttpPatch("{packId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePack(
        Guid packId,
        [FromBody] UpdatePackRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await packService.Update(userId, packId, request);

        if (result.TryGetFault(out var error, out var pack))
            return BadRequest(error);
        return Ok(pack);
    }

    
    [HttpDelete("{packId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePack(Guid packId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await packService.Delete(userId, packId);
        if (result.TryGetFault(out var error, out var track))
            return NotFound(error);
        return NoContent();
    }

    [HttpGet("{packId:guid}/edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    public async Task<IActionResult> GetPack(Guid packId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await packService.GetPack(userId, packId);
        if (!result.TryGetFault(out var error, out var pack)) return Ok(ToResponse(packId, pack));
        var created = await packService.Create(userId, new CreatePackRequest
        {
            PackId = packId,
            Name = string.Empty,
            Rounds =
            [
                new CreateRoundRequest
                {
                    Name = "Тур 1",
                    Questions =
                    [
                        [
                            new RoundQuestionRequest
                            {
                                Cost = 100,
                                Category = "Первая",
                                QuestionId = null
                            }
                        ]
                    ]
                }
            ]
        });

        if (created.TryGetFault(out var createError, out _))
            return BadRequest(createError);

        result = await packService.GetPack(userId, packId);
        if (result.TryGetFault(out error, out pack))
            return NotFound(error);

        return Ok(ToResponse(packId, pack));
    }
    
    private static EditPackResponse ToResponse(Guid packId, Pack pack) => new()
    {
        PackId = packId,
        Name = pack.Name,
        Rounds = pack.Rounds.Select(ToRoundResponse).ToList()
    };

    private static RoundResponse ToRoundResponse(Round round)
    {
        var field = round.Field ?? [];
        var categories = (round.Categories ?? [])
            .Concat(field.Keys)
            .Distinct()
            .ToList();
        var costs = field.Values
            .SelectMany(row => row.Keys)
            .Distinct()
            .Order()
            .ToList();

        return new RoundResponse
        {
            Name = round.Name,
            Categories = categories,
            Costs = costs,
            Questions = categories
                .Select(category => costs
                    .Select(cost =>
                    {
                        Question? question = null;
                        if (field.TryGetValue(category, out var row))
                            row.TryGetValue(cost, out question);
                        return new QuestionResponse
                        {
                            QuestionId = question?.Id,
                            Category = category,
                            Cost = cost,
                            CorrectAnswer = question?.CorrectAnswer,
                        };
                    })
                    .ToList())
                .ToList()
        };
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out userId);
    }
}