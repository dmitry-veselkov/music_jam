using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Storages;
using Shared;

namespace MusicJam.Studio.Services.PackService;

public class PackService(
    IPackStorage packStorage, IQuestionStorage questionStorage)
    : IPackService
{
    public async Task<List<(Guid, string)>> GetShort(Guid userId, GetPackRequest query)
    {
        var packs = await packStorage.GetPacksByUser(userId, query.Take, query.Skip);
        return packs
            .Select(x => (x.Id, x.Name))
            .ToList();
    }
    public async Task<Result<Pack, IPackService.PackError>> Create(Guid userId, CreatePackRequest request)
    {
        var rounds = await GetAllRounds(userId, request);
        var pack = await packStorage.SavePack(userId, new Pack()
        {
            Id = request.PackId,
            Rounds = rounds ?? [],
            Name = request.Name,
        });
        return pack;
    }

    public async Task<Result<Pack, IPackService.PackError>> Update(Guid userId, Guid packId, UpdatePackRequest request)
    {
        var rounds = await GetAllRounds(userId, request);
        var pack = await packStorage.Update(userId, new Pack()
        {
            Id = packId,
            Rounds = rounds,
            Name = request.Name,
        });
        return pack;
    }

    public async Task<Result<bool, IPackService.PackError>> Delete(Guid userId, Guid packId)
    {
        var deleted = await packStorage.DeletePack(userId, packId);
        if (!deleted)
            return IPackService.PackError.NotFound;
        return true;
    }

    public async Task<Result<Pack, IPackService.PackError>> GetPack(Guid userId, Guid packId)
    {
        var pack = await packStorage.GetPackByUser(userId, packId);
        if (pack is null)
            return IPackService.PackError.NotFound;
        return pack;
    }
    
    public async Task<List<Pack>> GetPacks(Guid userId, GetPackRequest query)
    {
        var packIds = await packStorage.GetPacksByUser(userId);
        return packIds
            .Skip(query.Skip)
            .Take(query.Take)
            .ToList();
    }

    private async Task<List<Round>?> GetAllRounds(Guid userId, CreatePackRequest request)
    {
        var ids = request.Rounds
            .SelectMany(r => r.Cells)
            .Where(q => q.QuestionId.HasValue)
            .Select(q => q.QuestionId!.Value)
            .Distinct()
            .ToList();

        var questions = new Dictionary<Guid, Question>();
        foreach (var id in ids)
        {
            var question = await questionStorage.GetQuestion(userId, id);
            questions[id] = question;
        }

        if (request.Rounds.Any(r =>
                r.Cells.GroupBy(q => (q.Category, q.Cost)).Any(g => g.Count() > 1)))
            return null;

        return request.Rounds.Select(r => new Round
        {
            Name = r.Name,
            Categories = r.Cells.Select(q => q.Category).Distinct().ToList(),
            Field = r.Cells
                .GroupBy(q => q.Category)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(q => q.Cost, q => q.QuestionId is { } id ? questions[id] : null))
        }).ToList();
    }
}