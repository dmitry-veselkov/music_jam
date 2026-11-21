using MusicJam.Runtime.Models;
using Shared;

namespace MusicJam.Runtime.Services.GameService;

public class GameServiceLoggerDecorator(
    IGameService gameService,
    ILogger<GameService> logger)
    : IGameService
{
    public async Task<Result<Game, IGameService.GameError>> Create(Guid hostId, Guid packId)
    {
        logger.LogInformation(
            "Starting new game creation. HostId: {HostId}, PackId: {PackId}",
            hostId,
            packId);

        var result = await gameService.Create(hostId, packId);
        if (result.TryGetFault(out var error, out var game))
        {
            logger.LogWarning(
                "Failed to create a new game. HostId: {HostId}, Error: {Error}",
                hostId,
                error);

            return error;
        }

        logger.LogInformation(
            "New game created successfully. GameId: {GameId}, HostId: {HostId}, PackId: {PackId}",
            game.GameId,
            hostId,
            packId);

        return game;
    }

    public async Task<Result<Game, IGameService.GameError>> Get(Guid gameId)
    {
        return await gameService.Get(gameId);
    }
}
