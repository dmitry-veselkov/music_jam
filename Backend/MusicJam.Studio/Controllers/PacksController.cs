using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests.Packs;
using MusicJam.Studio.Responses;
using MusicJam.Studio.Responses.Packs;
using MusicJam.Studio.Services.PacksService;

namespace MusicJam.Studio.Controllers;

[ApiController]
[Route("api/packs")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
public class PacksController(IPacksService packsService) : ControllerBase
{
    [HttpGet("short")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShortPackResponse[]>> GetShortPacks([FromQuery] GetPackRequest request)
    {
        var result = await packsService.GetShortPacks(User.GetUserId(), request);
        if (result.TryGetFault(out var error, out var shortPacks))
        {
            return ErrorResponse(error);
        }

        return shortPacks;
    }

    [HttpGet("cards")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PackCardResponse[]>> GetCardPacks([FromQuery] GetPackRequest query)
    {
        var userId = User.GetUserId();

        var result = await packsService.GetPackCards(userId, query);
        if (result.TryGetFault(out var error, out var packs))
        {
            return ErrorResponse(error);
        }

        return packs;
    }
    
    [HttpGet("{packId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pack>> GetPack(Guid packId)
    {
        var result = await packsService.GetPack(User.GetUserId(), packId);
        if (result.TryGetFault(out var error, out var pack))
        {
            return ErrorResponse(error);
        }

        return pack;
    }

    /// <summary>
    /// Полный пак со всеми раундами и вопросами. Используется Runtime при старте игры.
    /// </summary>
    [HttpGet("{packId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pack>> GetPack(Guid packId)
    {
        var result = await packsService.GetPack(User.GetUserId(), packId);
        if (result.TryGetFault(out var error, out var pack))
        {
            return ErrorResponse(error);
        }

        return pack;
    }

    [HttpGet("{packId:guid}/edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EditPackResponse>> GetPackForEdit(Guid packId)
    {
        var userId = User.GetUserId();

        var result = await packsService.GetEditPack(userId, packId);
        if (result.TryGetFault(out var error, out var packForEdit))
        {
            return ErrorResponse(error);
        }

        return packForEdit;
    }

    [HttpPatch("{packId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pack>> UpdatePack(Guid packId, [FromBody] UpdatePackRequest request)
    {
        var userId = User.GetUserId();

        var result = await packsService.Update(userId, packId, request);
        if (result.TryGetFault(out var error, out var pack))
        {
            return ErrorResponse(error);
        }

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

        var result = await packsService.Delete(userId, packId);
        if (result.TryGetFault(out var error, out _))
        {
            return ErrorResponse(error);
        }

        return NoContent();
    }

    private ActionResult ErrorResponse(IPacksService.PackError error)
    {
        return error switch
        {
            IPacksService.PackError.NotFound => NotFound(),
            IPacksService.PackError.InvalidRequest => BadRequest(),
            IPacksService.PackError.DuplicateCell => BadRequest(),
            IPacksService.PackError.QuestionNotFound => BadRequest(),
            IPacksService.PackError.UnexpectedError => StatusCode(StatusCodes.Status500InternalServerError),
            _ => throw new ArgumentOutOfRangeException(error.ToString()),
        };
    }
}
