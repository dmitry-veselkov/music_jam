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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetShortPack([FromQuery] GetPackRequest query)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var result = await packService.GetShort(userId, query);
        return Ok(new { result });
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
        return Ok(track);
    }

    [HttpGet("{packId:guid}/edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    public async Task<IActionResult?> GetPack(Guid packId)
    {
        EditPackResponse dto;
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await packService.GetPack(userId, packId);
        if (result.TryGetFault(out var error, out var pack))
        {
            dto = new EditPackResponse()
            {
                PackId = packId,
                Name = string.Empty,
                Rounds = []
            };
        }
        dto = new EditPackResponse
        {
            PackId = packId,
            Name = pack.Name,

            Rounds = pack.Rounds.Select(round => new RoundResponse
            {
                Name = round.Name,

                Categories = round.Field.Keys.ToList(),

                Costs = round.Field.Count > 0
                    ? round.Field.First().Value.Keys
                        .OrderBy(cost => cost)
                        .ToList()
                    : [],

                Questions = round.Field.Select(category =>
                    category.Value
                        .OrderBy(question => question.Key)
                        .Select(question => new QuestionResponse
                        {
                            QuestionId = question.Value.Id,
                            Category = category.Key,
                            Cost = question.Key,
                            CorrectAnswer = question.Value.CorrectAnswer,
                            Requested = question.Value.Requested
                        })
                        .ToList()
                ).ToList()
            }).ToList()
        };

        return Ok(dto);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out userId);
    }
}