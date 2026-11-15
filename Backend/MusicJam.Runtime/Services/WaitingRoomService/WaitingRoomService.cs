using MusicJam.Runtime.Models;
using MusicJam.Runtime.Responses;
using MusicJam.Runtime.Storages.ConnectionStorage;
using MusicJam.Runtime.Storages.GameStorage;
using Shared;
using WaitingRoomError = MusicJam.Runtime.Services.WaitingRoomService.IWaitingRoomService.WaitingRoomError;

namespace MusicJam.Runtime.Services.WaitingRoomService;

public sealed class WaitingRoomService(
    IGameStorage gameStorage,
    IConnectionStorage connectionStorage) : IWaitingRoomService
{
    public async Task<Result<JoinResponse, WaitingRoomError>> Join(
        string connectionId,
        Guid sessionId,
        Guid gameId,
        Role role,
        string? teamName)
    {
        if (role is not (Role.Player or Role.Viewer))
        {
            return WaitingRoomError.InvalidRole;
        }

        var game = await gameStorage.Get(gameId);
        if (game is null)
        {
            return WaitingRoomError.GameNotFound;
        }

        if (await connectionStorage.Get(connectionId) is not null)
        {
            return WaitingRoomError.AlreadyInRoom;
        }

        var result = role == Role.Viewer
            ? JoinAsViewer(game)
            : JoinAsPlayer(game, sessionId, teamName);

        if (result.IsSuccess)
        {
            await connectionStorage.Bind(connectionId, new ConnectionBinding(gameId, sessionId, role));
        }

        return result;
    }

    private static Result<JoinResponse, WaitingRoomError> JoinAsViewer(Game game)
    {
        // Зритель может прийти в любой момент игры, команда ему не нужна.
        lock (game.Lock)
        {
            return new JoinResponse
            {
                Team = null,
                Teams = game.Teams.Values.ToArray(),
                IsNewTeam = false,
            };
        }
    }

    private static Result<JoinResponse, WaitingRoomError> JoinAsPlayer(Game game, Guid sessionId, string? teamName)
    {
        lock (game.Lock)
        {
            var existing = game.Teams.Values.FirstOrDefault(t => t.SessionId == sessionId);
            if (existing is not null)
            {
                existing.State = TeamState.Ready;
                return new JoinResponse
                {
                    Team = existing,
                    Teams = game.Teams.Values.ToArray(),
                    IsNewTeam = false,
                };
            }

            if (game.State != GameState.Pending)
            {
                return WaitingRoomError.GameAlreadyStarted;
            }

            var name = teamName?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                return WaitingRoomError.EmptyName;
            }

            if (name.Length > IWaitingRoomService.MaxTeamNameLength)
            {
                return WaitingRoomError.LongName;
            }

            if (game.Teams.Values.Any(t => string.Equals(t.TeamName, name, StringComparison.OrdinalIgnoreCase)))
            {
                return WaitingRoomError.NameTaken;
            }

            var team = new Team
            {
                TeamId = Guid.NewGuid(),
                TeamName = name,
                State = TeamState.Ready,
                SessionId = sessionId,
            };
            game.Teams.Add(team.TeamId, team);

            return new JoinResponse
            {
                Team = team,
                Teams = game.Teams.Values.ToArray(),
                IsNewTeam = true,
            };
        }
    }

    public async Task<Result<WaitingRoomError>> Leave(string connectionId, Guid gameId, Guid teamId)
    {
        var binding = await connectionStorage.Get(connectionId);
        if (binding is null || binding.GameId != gameId || binding.Role != Role.Player)
        {
            return WaitingRoomError.NotYourTeam;
        }

        var game = await gameStorage.Get(gameId);
        if (game is null)
        {
            return WaitingRoomError.GameNotFound;
        }

        lock (game.Lock)
        {
            if (!game.Teams.TryGetValue(teamId, out var team) || team.SessionId != binding.ParticipantId)
            {
                return WaitingRoomError.NotYourTeam;
            }

            game.Teams.Remove(teamId);
        }

        await connectionStorage.Unbind(connectionId);
        return Result<WaitingRoomError>.Success();
    }

    public async Task<RemovedTeam?> Disconnect(string connectionId)
    {
        var binding = await connectionStorage.Unbind(connectionId);
        if (binding is null)
        {
            return null;
        }

        // Зритель уходит молча: команды у него нет.
        if (binding.Role != Role.Player || await connectionStorage.HasAny(binding))
        {
            return null;
        }

        var game = await gameStorage.Get(binding.GameId);
        if (game is null)
        {
            return null;
        }

        lock (game.Lock)
        {
            var team = game.Teams.Values.FirstOrDefault(t => t.SessionId == binding.ParticipantId);
            if (team is null)
            {
                return null;
            }

            if (game.State != GameState.Pending)
            {
                team.State = TeamState.Disconnected;
                return null;
            }

            game.Teams.Remove(team.TeamId);
            return new RemovedTeam(game.GameId, team.TeamId);
        }
    }
}
