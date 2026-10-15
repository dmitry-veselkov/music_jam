using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Requests;
using MusicJam.Studio.Models.Responses;
using MusicJam.Studio.Storages.PacksStorage;
using MusicJam.Studio.Storages.QuestionsStorage;
using Shared;

namespace MusicJam.Studio.Services.PacksService;

internal class PacksService(
    IPacksStorage packsStorage,
    IQuestionsStorage questionsStorage)
    : IPacksService
{
    public async Task<Result<PackCard[], IPacksService.PacksError>> GetPackCards(
        Guid userId,
        PacksWithPaginationRequest request)
    {
        return await packsStorage.GetPackCards(userId, request.Skip, request.Take);
    }

    public async Task<Result<PackShort[], IPacksService.PacksError>> GetPacksShort(
        Guid userId,
        PacksWithPaginationRequest request)
    {
        return await packsStorage.GetPacksShort(userId, request.Skip, request.Take);
    }

    public async Task<Result<PackEdit, IPacksService.PacksError>> GetPackForEdit(
        Guid userId,
        Guid packId)
    {
        var pack = await packsStorage.GetPackById(userId, packId);
        if (pack is null)
        {
            return IPacksService.PacksError.PackNotFound;
        }

        return await ConvertPackToEdit(pack);
    }

    public async Task<Result<IPacksService.PacksError>> CreatePack(
        Guid userId,
        LoadPackRequest request)
    {
        // todo veselkov.d добавить валидацию входных данных

        var now = DateTime.UtcNow;
        var rounds = request.Rounds
            .Select(ConvertRoundRequestToRound)
            .ToList();

        var pack = new Pack
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            Rounds = rounds,
            Usages = []
        };

        await packsStorage.SavePack(userId, pack);

        return Result<IPacksService.PacksError>.Success();
    }

    public async Task<Result<IPacksService.PacksError>> DeletePack(
        Guid userId,
        Guid packId)
    {
        var isPackInUse = await packsStorage.IsPackInUse(userId, packId);
        if (isPackInUse)
        {
            return IPacksService.PacksError.PackIsInUse;
        }

        await packsStorage.DeletePack(userId, packId);

        return Result<IPacksService.PacksError>.Success();
    }

    public async Task<Result<IPacksService.PacksError>> UpdatePack(Guid userId, Guid packId, LoadPackRequest request)
    {
        // todo veselkov.d реализовать обновление существующего пака
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    private async Task<PackEdit> ConvertPackToEdit(Pack pack)
    {
        var allQuestionIds = pack.Rounds
            .SelectMany(round => FlattenQuestionIds(round.QuestionIds))
            .Distinct()
            .ToList();

        var correctAnswers =
            allQuestionIds.Count == 0
                ? new Dictionary<Guid, string>()
                : await questionsStorage.GetCorrectAnswers(allQuestionIds);

        var rounds = pack.Rounds
            .Select(round => ConvertRoundToEdit(round, correctAnswers))
            .ToList();

        return new PackEdit
        {
            Id = pack.Id,
            Name = pack.Name,
            Rounds = rounds
        };
    }

    private static RoundEdit ConvertRoundToEdit(Round round, Dictionary<Guid, string> correctAnswers)
    {
        var categories = round.QuestionIds.Keys.ToList();

        var costs = categories.Count == 0
            ? []
            : round.QuestionIds[categories[0]].Keys
                .OrderBy(cost => cost)
                .ToList();

        return new RoundEdit
        {
            Name = round.Name,
            Categories = categories,
            Costs = costs,
            Questions = round.QuestionIds
                .Select(row => row.Value
                    .OrderBy(column => column.Key)
                    .Select(column => GetQuestionEditCell(
                        row.Key,
                        column.Key,
                        column.Value,
                        correctAnswers))
                    .ToList())
                .ToList()
        };
    }

    private static Round ConvertRoundRequestToRound(RoundRequest request)
    {
        return new Round
        {
            Name = request.Name,
            QuestionIds = request.Questions
                .SelectMany(row => row)
                .GroupBy(cell => cell.Category)
                .ToDictionary(
                    category => category.Key,
                    category => category.ToDictionary(
                        cell => cell.Cost,
                        cell => cell.QuestionId))
        };
    }

    private static QuestionEditCell GetQuestionEditCell(
        string category,
        int cost,
        Guid questionId,
        Dictionary<Guid, string> correctAnswers)
    {
        if (!correctAnswers.TryGetValue(questionId, out var correctAnswer))
        {
            throw new InvalidOperationException(
                $"Correct answer for question '{questionId}' was not found.");
        }

        return new QuestionEditCell
        {
            Category = category,
            Cost = cost,
            QuestionId = questionId,
            CorrectAnswer = correctAnswer
        };
    }

    private static IEnumerable<Guid> FlattenQuestionIds(Dictionary<string, Dictionary<int, Guid>> questionIds)
    {
        return questionIds.Values
            .SelectMany(questionsByCost => questionsByCost.Values);
    }
}
