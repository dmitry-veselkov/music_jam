using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;
using MusicJam.Studio.Models.Requests;
using MusicJam.Studio.Models.Responses;
using MusicJam.Studio.Services.PacksService;

namespace MusicJam.Studio.Controllers;

[ApiController]
[Route("api/packs")]
[Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
public class PacksController(IPacksService packsService) : ControllerBase
{
    [HttpGet("cards")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackCard[]))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPackCards([FromQuery] PacksWithPaginationRequest withPaginationRequest)
    {
        var userId = User.GetUserId();

        var packCardsResult = await packsService.GetPackCards(userId, withPaginationRequest);
        if (packCardsResult.TryGetFault(out var error, out var packCards))
        {
            return ErrorResponse(error);
        }

        return Ok(packCards);
    }

    [HttpGet("short")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackShort[]))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPacksShort([FromQuery] PacksWithPaginationRequest withPaginationRequest)
    {
        var userId = User.GetUserId();

        var packsShortResult = await packsService.GetPacksShort(userId, withPaginationRequest);
        if (packsShortResult.TryGetFault(out var error, out var packsShort))
        {
            return ErrorResponse(error);
        }

        return Ok(packsShort);
    }

    [HttpGet("{packId:guid}/edit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackEdit))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPackForEdit([FromRoute] Guid packId)
    {
        var userId = User.GetUserId();

        var packEditResult = await packsService.GetPackForEdit(userId, packId);
        if (packEditResult.TryGetFault(out var error, out var packEdit))
        {
            return ErrorResponse(error);
        }

        return Ok(packEdit);
    }

    [HttpDelete("{packId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePack([FromRoute] Guid packId)
    {
        var userId = User.GetUserId();

        var deletePackResult = await packsService.DeletePack(userId, packId);
        if (deletePackResult.TryGetFault(out var error))
        {
            return ErrorResponse(error);
        }

        return NoContent();
    }

    private IActionResult ErrorResponse(IPacksService.PacksError error)
    {
        return error switch
        {
            IPacksService.PacksError.PackNotFound => NotFound(),
            IPacksService.PacksError.PackIsInUse => BadRequest(),
            _ => throw new ArgumentOutOfRangeException(error.ToString()),
        };
    }
}
