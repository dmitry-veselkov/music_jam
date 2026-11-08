using MusicJam.Runtime.Models;
using MusicJam.Runtime.Responses;
using MusicJam.Runtime.Storages.ConnectionStorage;
using MusicJam.Runtime.Storages.GameStorage;
using MusicJam.Runtime.Storages.TeamStorage;
using Shared;
using WaitingRoomError = MusicJam.Runtime.Services.WaitingRoomService.IWaitingRoomService.WaitingRoomError;

namespace MusicJam.Runtime.Services.WaitingRoomService;

public sealed class WaitingRoomService(
    IGameStorage gameStorage,
    ITeamStorage teamStorage,
    IConnectionStorage connectionStorage) : IWaitingRoomService
{
    public async Task<Result<JoinResponse, WaitingRoomError>> Join(
        string connectionId,
        Guid gameId,
        string teamName)
    {
        var name = teamName?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            return WaitingRoomError.EmptyName;
        }

        if (name.Length > IWaitingRoomService.MaxTeamNameLength)
        {
            return WaitingRoomError.LongName;
        }

        if (await connectionStorage.Get(connectionId) is not null)
        {
            return WaitingRoomError.AlreadyInRoom;
        }

        await gameStorage.GetOrCreate(gameId);

        var team = new Team
        {
            TeamId = Guid.NewGuid(),
            TeamName = name,
            State = TeamState.Ready,
        };

        // Названия команд уникальны без учёта регистра
        var nameKey = name.ToUpperInvariant();
        if (!await teamStorage.TryAdd(gameId, nameKey, team))
        {
            return WaitingRoomError.NameTaken;
        }

        await connectionStorage.Bind(connectionId, new ConnectionBinding(gameId, team.TeamId));

        return new JoinResponse
        {
            Team = team,
            Teams = await teamStorage.GetTeams(gameId),
        };
    }

    public async Task<Result<WaitingRoomError>> Leave(string connectionId, Guid gameId, Guid teamId)
    {
        var binding = await connectionStorage.Get(connectionId);
        if (binding is null || binding.GameId != gameId || binding.TeamId != teamId)
        {
            return WaitingRoomError.NotYourTeam;
        }

        await connectionStorage.Unbind(connectionId);
        await teamStorage.Remove(gameId, teamId);

        return Result<WaitingRoomError>.Success();
    }

    public async Task<ConnectionBinding?> Disconnect(string connectionId)
    {
        var binding = await connectionStorage.Unbind(connectionId);
        if (binding is null)
        {
            return null;
        }

        var removed = await teamStorage.Remove(binding.GameId, binding.TeamId);
        return removed is null ? null : binding;
    }
}
