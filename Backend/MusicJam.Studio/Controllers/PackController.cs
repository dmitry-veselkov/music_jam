using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Responses;
using MusicJam.Studio.Services.PackService;

namespace MusicJam.Studio.Controllers;

[ApiController]
[Route("api/packs")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
public class PackController(IPackService packService, IMapper mapper) : ControllerBase
{
   
    [HttpGet("short")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ShortPackResponse>>> GetShortPack([FromQuery] GetPackRequest query)
    {
        var result = await packService.GetShort(userId, query);
        return result
            .Select(x => new ShortPackResponse { PackId = x.Item1, Name = x.Item2 })
            .ToList();
    }
    
    [HttpGet("cards")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetPackResponse>>> GetCardPacks([FromQuery] GetPackRequest query)
    {
        var userId = User.GetUserId();
        var result = await packService.GetPacks(userId, query);
        return result
            .Select(x => x.ToCardResponse())
            .ToList();
    }
    

    [HttpPatch("{packId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pack>> UpdatePack(
        Guid packId,
        [FromBody] UpdatePackRequest request)
    {
        var userId = User.GetUserId();
        var result = await packService.Update(userId, packId, request);

        if (result.TryGetFault(out var error, out var pack))
            return ErrorResponse(error);
        return pack;
    }

    
    [HttpDelete("{packId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePack(Guid packId)
    {
        var userId = User.GetUserId();
        var result = await packService.Delete(userId, packId);
        if (result.TryGetFault(out var error, out var track))
            return ErrorResponse(error);
        return NoContent();
    }

    [HttpGet("{packId:guid}/edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPack(Guid packId)
    {
        var userId = User.GetUserId();
        var result = await packService.GetPack(userId, packId);
        if (!result.TryGetFault(out var error, out var pack))
        {
            return Ok(pack.ToEditResponse());
        }
        var created = await packService.Create(userId, new CreatePackRequest
        {
            PackId = packId,
            Name = string.Empty,
            Rounds =
            [
                new CreateRoundRequest
                {
                    Questions = []
                }
            ]
        });

        if (created.TryGetFault(out var createError, out _))
            return ErrorResponse(createError);

        result = await packService.GetPack(userId, packId);
        if (result.TryGetFault(out error, out pack))
            return ErrorResponse(error);

        return Ok(pack.ToEditResponse());
    }
}