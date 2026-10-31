using MusicJam.Studio.Extensions;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Requests.Packs;
using MusicJam.Studio.Responses;
using MusicJam.Studio.Responses.Packs;
using MusicJam.Studio.Storages;
using Shared;

namespace MusicJam.Studio.Services.PackService;

public class PacksService(
    IPackStorage packStorage,
    IQuestionStorage questionStorage)
    : IPacksService
{
    public async Task<Result<ShortPackResponse[], IPacksService.PackError>> GetShortPacks(
        Guid userId,
        GetPackRequest request)
    {
        // todo Elizaveta-Vinogradova
        // сейчас, чтобы получить лишь список названий, мы выгружаем из БД весь пак целиком
        // нужно использовать представления и сразу брать только то, что действительно нужно
        var packs = await packStorage.GetPacksByUser(userId, request.Take, request.Skip);

        return packs
            .Select(x => x.ToShortResponse())
            .ToArray();
    }

    public async Task<Result<PackCardResponse[], IPacksService.PackError>> GetPackCards(
        Guid userId,
        GetPackRequest request)
    {
        // todo Elizaveta-Vinogradova
        // для карточек паков нам не нужна информация о турах и заполненности их таблиц,
        // поэтому здесь тоже, наверное, нужно использовать представления
        var packs = await packStorage.GetPacksByUser(userId, request.Take, request.Skip);

        return packs
            .Select(pack => pack.ToCardResponse())
            .ToArray();
    }

    public async Task<Result<EditPackResponse, IPacksService.PackError>> GetEditPackOrCreate(Guid userId, Guid packId)
    {
        var getResult = await GetPack(userId, packId);
        if (getResult.TryGetValue(out var pack, out var getPackError))
        {
            return pack.ToEditResponse();
        }

        if (getPackError != IPacksService.PackError.NotFound)
        {
            return getPackError;
        }

        var emptyPackRequest = CreateEmptyPackRequest(packId);
        var createResult = await Create(userId, emptyPackRequest);

        return createResult.GetValueOrThrow().ToEditResponse();
    }

    public async Task<Result<Pack, IPacksService.PackError>> GetPack(Guid userId, Guid packId)
    {
        var pack = await packStorage.GetPackByUser(userId, packId);
        if (pack is null)
        {
            return IPacksService.PackError.NotFound;
        }

        return pack;
    }

    public async Task<Result<Pack, IPacksService.PackError>> Create(Guid userId, CreatePackRequest request)
    {
        var rounds = await GetAllRounds(userId, request);

        var pack = await packStorage.SavePack(userId, new Pack
        {
            Id = request.PackId,
            Rounds = rounds,
            Name = request.Name,
        });

        return pack;
    }

    public async Task<Result<Pack, IPacksService.PackError>> Update(Guid userId, Guid packId, UpdatePackRequest request)
    {
        var rounds = await GetAllRounds(userId, request);
        var pack = await packStorage.Update(userId, new Pack
        {
            Id = packId,
            Rounds = rounds,
            Name = request.Name,
        });

        return pack;
    }

    public async Task<Result<bool, IPacksService.PackError>> Delete(Guid userId, Guid packId)
    {
        var isDeleted = await packStorage.DeletePack(userId, packId);
        if (!isDeleted)
        {
            return IPacksService.PackError.NotFound;
        }

        return true;
    }

    // todo Elizaveta-Vinogradova
    // с этим методом явно что-то надо сделать, ибо его даже читать невозможно,
    // а про редактирование и дебаг я вообще молчу
    private async Task<List<Round>> GetAllRounds(Guid userId, CreatePackRequest request)
    {
        var ids = request.Rounds
            .SelectMany(r => r.Cells)
            .Where(q => q.QuestionId.HasValue)
            .Select(q => q.QuestionId!.Value)
            .Distinct()
            .ToList();

        var questions = new Dictionary<Guid, Question?>();
        foreach (var id in ids)
        {
            var question = await questionStorage.GetQuestion(userId, id);
            questions[id] = question;
        }

        if (request.Rounds.Any(r =>
                r.Cells.GroupBy(q => (q.Category, q.Cost)).Any(g => g.Count() > 1)))
        {
            return [];
        }

        return request.Rounds.Select(r => new Round
        {
            Name = r.Name,
            Categories = r.Cells.Select(q => q.Category).Distinct().ToList(),
            Field = r.Cells
                .GroupBy(q => q.Category)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(
                        q => q.Cost,
                        q => q.QuestionId is { } id ? questions[id] : null))
        }).ToList();
    }

    private static CreatePackRequest CreateEmptyPackRequest(Guid packId)
    {
        return new CreatePackRequest
        {
            PackId = packId,
            Name = string.Empty,
            Rounds =
            [
                new CreateRoundRequest
                {
                    Questions = []
                }
            ]
        };
    }
}
