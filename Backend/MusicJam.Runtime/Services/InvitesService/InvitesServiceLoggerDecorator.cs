using MusicJam.Runtime.Models;
using Shared;

namespace MusicJam.Runtime.Services.InvitesService;

internal class InvitesServiceLoggerDecorator(
    ILogger<InvitesService> logger,
    InvitesService invitesService)
    : IInvitesService
{
    public async Task<GameInvite> CreateInvite(Guid gameId)
    {
        logger.LogInformation(
            "Starting new invite creation. GameId: {GameId}",
            gameId);

        var invite = await invitesService.CreateInvite(gameId);

        logger.LogInformation(
            "Invite created successfully. GameId: {GameId}, InviteId: {InviteId}, Code: {Code}",
            gameId,
            invite.InviteId,
            invite.Code);

        return invite;
    }

    public async Task<Result<string, IInvitesService.InviteError>> JoinGameByCode(string code)
    {
        logger.LogInformation(
            "Attempting to join game with code. Code: {Code}",
            code);

        var result = await invitesService.JoinGameByCode(code);
        if (result.TryGetFault(out var error, out var backUrl))
        {
            logger.LogWarning(
                "Failed to join game with code. Code: {Code}, Error: {Error}",
                code,
                error);

            return error;
        }

        logger.LogInformation(
            "Successfully joined game with code. Code: {Code}, BackUrl: {BackUrl}",
            code,
            backUrl);

        return backUrl;
    }

    public async Task<Result<string, IInvitesService.InviteError>> JoinGameByLink(Guid inviteId)
    {
        logger.LogInformation(
            "Attempting to join game with link. InviteId: {InviteId}",
            inviteId);

        var result = await invitesService.JoinGameByLink(inviteId);
        if (result.TryGetFault(out var error, out var backUrl))
        {
            logger.LogWarning(
                "Failed to join game with link. InviteId: {InviteId}, Error: {Error}",
                inviteId,
                error);

            return error;
        }

        logger.LogInformation(
            "Successfully joined game with link. InviteId: {InviteId}, BackUrl: {BackUrl}",
            inviteId,
            backUrl);

        return backUrl;
    }
}
