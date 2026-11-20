using Microsoft.AspNetCore.Mvc;
using MusicJam.Runtime.Clients.StudioClient;
using MusicJam.Runtime.Services.InviteService;
using MusicJam.Runtime.Storages.GameStorage;
using Shared;
using Game = MusicJam.Runtime.Models.Game;

namespace MusicJam.Runtime.Services.GameService;

public class GameService(
    IStudioClient studioClient,
    IGameStorage gameStorage,
    IInviteService inviteService) : IGameService
{
    //сюда потом взаимодействия по раундам
    public async Task<Result<Game, IGameService.GameError>> Create(Guid hostId, Guid packId)
    {
        var pack = await studioClient.GetPack(packId);
        if (pack is null)
        {
            return IGameService.GameError.GameNotFound;
        }

        var gameId = Guid.NewGuid();
        var inviteInfo = inviteService.CreateInviteInfo(gameId);

        var newGame = new Game
        {
            GameId = gameId,
            HostId = hostId,
            Pack = pack,
            InviteInfo = inviteInfo
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