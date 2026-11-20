using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using MusicJam.Runtime.Models;
using MusicJam.Runtime.Storages.InvitesStorage;
using Shared;

namespace MusicJam.Runtime.Services.InviteService;

internal class InviteService(
    IOptions<EnvironmentSettings> environmentSettings,
    IInvitesStorage invitesStorage)
    : IInviteService
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

    public async Task<Result<string, IInviteService.InviteError>> JoinGameByCode(string code)
    {
        if (!IsValidCode(code))
        {
            return IInviteService.InviteError.InvalidCode;
        }

        var invite = await invitesStorage.GetInviteByCode(code);
        if (invite is null)
        {
            return IInviteService.InviteError.InviteNotFound;
        }

        return BuildWaitingRoomUrl(invite.GameId);
    }

    public async Task<Result<string, IInviteService.InviteError>> JoinGameByLink(Guid inviteId)
    {
        if (inviteId == Guid.Empty)
        {
            return IInviteService.InviteError.InvalidLink;
        }

        var invite = await invitesStorage.GetInviteById(inviteId);
        if (invite is null)
        {
            return IInviteService.InviteError.InviteNotFound;
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
        return $"{BaseUrl}/api/invites/join-game/by-link/{inviteId}";
    }
}