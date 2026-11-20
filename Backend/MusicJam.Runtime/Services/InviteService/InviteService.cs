using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using MusicJam.Runtime.Models;
using Shared;

namespace MusicJam.Runtime.Services.InviteService;

internal class InviteService(
    IOptions<EnvironmentSettings> environmentSettings)
    : IInviteService
{
    private const int CodeLength = 6;
    private const string CodeAlphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    public GameInviteInfo CreateInviteInfo(Guid gameId)
    {
        return new GameInviteInfo
        {
            Code = GenerateCode(),
            Link = GenerateLink(gameId),
        };
    }

    private static string GenerateCode()
    {
        return RandomNumberGenerator.GetString(CodeAlphabet, CodeLength);
    }

    private string GenerateLink(Guid gameId)
    {
        var baseUrl = environmentSettings.Value.BaseUrl;
        return $"{baseUrl}/games/{gameId}/waiting-room";
    }
}