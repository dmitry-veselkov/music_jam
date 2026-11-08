using MusicJam.Studio.Extensions;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Requests.Packs;
using MusicJam.Studio.Responses.Packs;
using MusicJam.Studio.Storages;
using MusicJam.Studio.Storages.QuestionStorage;
using Shared;

namespace MusicJam.Studio.Services.PacksService;

public class PacksService(
    IPackStorage packStorage,
    IQuestionStorage questionStorage)
    : IPacksService
{
    public async Task<Result<ShortPackResponse[], IPacksService.PackError>> GetShortPacks(
        Guid userId,
        GetPackRequest request)
    {
        var packs = await packStorage.GetShortPacksByUser(userId, request.Take, request.Skip);

        return packs
            .Select(x => new ShortPackResponse()
            {
                PackId = x.Id,
                Name = x.Name
            })
            .ToArray();
    }

    public async Task<Result<PackCardResponse[], IPacksService.PackError>> GetPackCards(
        Guid userId,
        GetPackRequest request)
    {
        var packs = await packStorage.GetCardPacksByUser(userId, request.Take, request.Skip);

        return packs
            .Select(x => new PackCardResponse()
            {
                PackId = x.Id,
                Name = x.Name,
                CreatedAtUtc = x.CreatedAt.ToString(),
                UpdatedAtUtc = x.UpdatedAt.ToString()
            })
            .ToArray();
    }

    public async Task<Result<EditPackResponse, IPacksService.PackError>> GetEditPack(Guid userId, Guid packId)
    {
        var result = await GetPack(userId, packId);
        if (result.TryGetFault(out var error, out var pack))
        {
            return error;
        }

        return pack.ToEditResponse();
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
        var roundsResult = await GetAllRounds(userId, request);
        if (roundsResult.TryGetFault(out var error, out var rounds))
        {
            return error;
        }
        
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
        var roundsResult = await GetAllRounds(userId, request);
        if (roundsResult.TryGetFault(out var error, out var rounds))
        {
            return error;
        }

        return await packStorage.Update(userId, new Pack
        {
            Id = packId,
            Rounds = rounds,
            Name = request.Name,
        });
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
    
    private async Task<Result<List<Round>, IPacksService.PackError>> GetAllRounds(Guid userId, CreatePackRequest request)
    {
        if (request.Rounds.Any(HasDuplicateCells))
            return IPacksService.PackError.DuplicateCell;

        var questionIds = CollectQuestionIds(request.Rounds);
        var questions = await questionStorage.GetQuestions(userId, questionIds);
        if (questions.Count != questionIds.Count)
            return IPacksService.PackError.QuestionNotFound;

        return request.Rounds
            .Select(r => BuildRound(r, questions))
            .ToList();
    }
    
    private static bool HasDuplicateCells(CreateRoundRequest round) =>
        round.Cells
            .GroupBy(c => (c.Category, c.Cost))
            .Any(g => g.Count() > 1);

    private static HashSet<Guid> CollectQuestionIds(IEnumerable<CreateRoundRequest> rounds) =>
        rounds
            .SelectMany(r => r.Cells)
            .Select(c => c.QuestionId)
            .OfType<Guid>()        
            .ToHashSet();

    private static Round BuildRound(CreateRoundRequest round, IReadOnlyDictionary<Guid, Question> questions)
    {
        var field = new Dictionary<string, Dictionary<int, Question?>>();

        foreach (var cell in round.Cells)
        {
            if (!field.TryGetValue(cell.Category, out var row))
                field[cell.Category] = row = new Dictionary<int, Question?>();

            row[cell.Cost] = cell.QuestionId is { } id ? questions[id] : null;
        }

        return new Round
        {
            Name = round.Name,
            Categories = round.Cells.Select(c => c.Category).Distinct().ToList(),
            Field = field
        };
    }
}
