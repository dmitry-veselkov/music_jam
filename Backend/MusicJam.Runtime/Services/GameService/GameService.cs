using Microsoft.AspNetCore.Mvc;
using MusicJam.Runtime.Clients.StudioClient;
using MusicJam.Runtime.Storages.GameStorage;
using Shared;
using Game = MusicJam.Runtime.Models.Game;

namespace MusicJam.Runtime.Services.GameService;

public class GameService(IStudioClient studioClient,
    IGameStorage gameStorage) : IGameService
{
    //сюда потом взаимодействия по раундам
    public async Task<Result<Game, IGameService.GameError>> Create(Guid hostId, Guid packId)
    {
        var pack = await studioClient.GetPack(packId);
        if (pack is null)
        {
            return IGameService.GameError.GameNotFound;
        }

        var newGame = new Game()
        {
            GameId = Guid.NewGuid(),
            HostId = hostId,
            Pack = pack,
        };
        await gameStorage.Add(newGame);
        return newGame;
    }

    public async Task<ActionResult<Game>> Get(Guid gameId)
    {
        var game = await gameStorage.Get(gameId);
        return game;
    }
}
