using MusicJam.Studio.Models.Games;
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

    public async Task<Result<Game, IGameService.GameError>> Create(Guid userId, CreateGameRequests query)
    {
        var game = new Game()
        {
            Id = query.packId,
            Description = query.Description ?? string.Empty,
            PlannedStart = query.Date ?? DateTime.Now,
            Mode = query.Mode
        };
        var gameTask = await gameStorage.SaveGame(userId, game);
        return gameTask;
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

    public async Task<Result<Game, IGameService.GameError>> Update(Guid gameId, UpdateGameRequests request)
    {
        var game = new Game()
        {
            Description = request.Description ?? string.Empty,
            PlannedStart = request.Date ?? DateTime.Now,
            Mode = request.Mode
        };
        var result = await gameStorage.UpdateGame(game);
        if (result == null)
            return IGameService.GameError.NotFound;
        return result;
    }
}