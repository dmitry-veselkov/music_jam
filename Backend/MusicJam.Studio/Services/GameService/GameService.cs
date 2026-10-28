using MusicJam.Studio.Models.Games;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Storages;
using Shared;

namespace MusicJam.Studio.Services.GameService;

public class GameService(
    IGameStorage gameStorage)
    : IGameService
{
    public async Task<Result<List<Game>, IGameService.GameError>> GetGames(Guid userId, GetGameRequests query)
    {
        var games = await gameStorage.GetAllUserGames(userId);
        Console.WriteLine(games.Count);
        return games
            .Skip(query.Skip)
            .Take(query.Take)
            .ToList();
    }

    public async Task<Result<Game, IGameService.GameError>> Create(Guid userId, CreateGameRequests request)
    {
        return await gameStorage.SaveGame(userId, ToModel(Guid.NewGuid(), request));
    }

    public async Task<Result<Game, IGameService.GameError>> Get(Guid gameId)
    {
        var game = await gameStorage.GetGame(gameId);
        if (game == null)
            return IGameService.GameError.NotFound;
        return game;
    }

    public async Task<Result<Guid, IGameService.GameError>> Delete(Guid gameId)
    {
        var result = await gameStorage.Delete(gameId);
        if (result == null)
            return IGameService.GameError.NotFound;
        return gameId;
    }

    public async Task<Result<Game, IGameService.GameError>> Update(Guid userId, Guid gameId, UpdateGameRequests request)
    {
        var game = ToModel(gameId, request);
        var updated = await gameStorage.UpdateGame(game);
        return updated ?? await gameStorage.SaveGame(userId, game);
    }

    private static Game ToModel(Guid gameId, CreateGameRequests request) => new()
    {
        Id = gameId,
        Name = request.Name,
        Description = request.Description ?? string.Empty,
        PlannedStart = request.Date ?? DateTime.UtcNow,
        Mode = request.Mode,
        State = State.Scheduled,
        Pack = new Pack { Id = request.PackId }
    };
}
