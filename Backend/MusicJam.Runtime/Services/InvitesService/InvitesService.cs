using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using MusicJam.Runtime.Models;
using MusicJam.Runtime.Storages.InvitesStorage;
using Shared;

namespace MusicJam.Runtime.Services.InvitesService;

internal class InvitesService(
    IOptions<EnvironmentSettings> environmentSettings,
    IInvitesStorage invitesStorage)
    : IInvitesService
{
    private string BaseUrl => environmentSettings.Value.BaseUrl;

    private const int CodeLength = 6;
    private const string CodeAlphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    public async Task<GameInvite> CreateInvite(Guid gameId)
    {
        var inviteId = Guid.NewGuid();

        var invite = new GameInvite
        {
            InviteId = inviteId,
            GameId = gameId,
            Code = GenerateCode(),
            Link = BuildInviteLink(inviteId),
        };

        await invitesStorage.SaveInvite(invite);

        return invite;
    }

    public async Task<Result<string, IInvitesService.InviteError>> JoinGameByCode(string code)
    {
        code = code.Trim().ToUpperInvariant();

        if (!IsValidCode(code))
        {
            return IInvitesService.InviteError.InvalidCode;
        }

        var invite = await invitesStorage.GetInviteByCode(code);
        if (invite is null)
        {
            return IInvitesService.InviteError.InviteNotFound;
        }

        return BuildWaitingRoomUrl(invite.GameId);
    }

    public async Task<Result<string, IInvitesService.InviteError>> JoinGameByLink(Guid inviteId)
    {
        if (inviteId == Guid.Empty)
        {
            return IInvitesService.InviteError.InvalidLink;
        }

        var invite = await invitesStorage.GetInviteById(inviteId);
        if (invite is null)
        {
            return IInvitesService.InviteError.InviteNotFound;
        }

        return BuildWaitingRoomUrl(invite.GameId);
    }

    private static bool IsValidCode(string code)
    {
        return code.Length == CodeLength && code.All(c => CodeAlphabet.Contains(c));
    }

    private static string GenerateCode()
    {
        return RandomNumberGenerator.GetString(CodeAlphabet, CodeLength);
    }

    private string BuildWaitingRoomUrl(Guid gameId)
    {
        return $"{BaseUrl}/games/{gameId}/waiting-room";
    }

    private string BuildInviteLink(Guid inviteId)
    {
        return $"{BaseUrl}/api/invites/join-game/by-link?inviteId={inviteId}";
    }
}
