using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
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
        var packDTO = result
            .Select(x => mapper.Map<GetPackResponse>(x))
            .ToList();
        return Ok(packDTO);
    }
    

    [HttpPost]
    [ProducesResponseType(typeof(Pack), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePack([FromBody] CreatePackRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var result = await packService.Create(userId, request);
        if (result.TryGetFault(out var error, out var pack))
            return BadRequest(error);
        return CreatedAtAction(nameof(GetPack), new { packId = pack.Id }, pack);
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
    public async Task<IActionResult> GetPack(Guid packId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await packService.GetPack(userId, packId);
        if (result.TryGetFault(out var error, out var pack))
            return NotFound(error);
        return Ok(pack);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out userId);
    }
}