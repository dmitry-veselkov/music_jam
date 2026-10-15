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

    public async Task<Result<PackForTable, IPacksService.PacksError>> GetPackForTable(Guid userId, Guid packId)
    {
        var pack = await packsStorage.GetPackById(userId, packId);
        if (pack is null)
        {
            return IPacksService.PacksError.PackNotFound;
        }

        return await ConvertPackToTableFormat(pack);
    }

    public async Task<Result<IPacksService.PacksError>> DeletePack(Guid userId, Guid packId)
    {
        var isPackInUse = await packsStorage.IsPackInUse(userId, packId);
        if (isPackInUse)
        {
            return IPacksService.PacksError.PackIsInUse;
        }

        await packsStorage.DeletePack(userId, packId);

        return Result<IPacksService.PacksError>.Success();
    }

    public async Task<Result<IPacksService.PacksError>> UpsertPack(Guid userId, Guid packId, LoadPackRequest request)
    {
        var validatePackResult = ValidatePackRequest(request);
        if (validatePackResult.TryGetFault(out var error))
        {
            return error;
        }

        var rounds = request.Rounds
            .Select(ConvertRoundRequestToRound)
            .ToList();

        var pack = await packsStorage.GetPackById(userId, packId);

        var updatedPack = pack is null
            ? CreateNewPack(request.Name, rounds)
            : UpdateExistPack(pack, request.Name, rounds);

        await packsStorage.SavePack(userId, updatedPack);

        return Result<IPacksService.PacksError>.Success();
    }

    private static Pack CreateNewPack(string name, List<Round> rounds)
    {
        var now = DateTime.UtcNow;

        return new Pack
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            Rounds = rounds,
            Usages = []
        };
    }

    private static Pack UpdateExistPack(Pack pack, string updatedName, List<Round> updatedRounds)
    {
        return new Pack
        {
            Id = pack.Id,
            Name = updatedName,
            CreatedAtUtc = pack.CreatedAtUtc,
            UpdatedAtUtc = DateTime.UtcNow,
            Rounds = updatedRounds,
            Usages = pack.Usages
        };
    }

    private static Result<IPacksService.PacksError> ValidatePackRequest(LoadPackRequest? request)
    {
        if (request is null)
        {
            return IPacksService.PacksError.EmptyRequest;
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return IPacksService.PacksError.EmptyPackName;
        }

        if (request.Rounds.Count == 0)
        {
            return IPacksService.PacksError.EmptyRounds;
        }

        foreach (var validateRoundResult in request.Rounds.Select(ValidateRoundRequest))
        {
            if (validateRoundResult.TryGetFault(out var roundError))
            {
                return roundError;
            }
        }

        return Result<IPacksService.PacksError>.Success();
    }

    private static Result<IPacksService.PacksError> ValidateRoundRequest(RoundRequest? round)
    {
        if (round is null)
        {
            return IPacksService.PacksError.InvalidRound;
        }

        if (string.IsNullOrWhiteSpace(round.Name))
        {
            return IPacksService.PacksError.EmptyRoundName;
        }

        var rowsCount = round.Questions.Count;

        if (rowsCount == 0)
        {
            return IPacksService.PacksError.EmptyQuestions;
        }

        var columnsCount = round.Questions.First().Count;

        if (round.Questions.Skip(1).Any(column => columnsCount != column.Count))
        {
            return IPacksService.PacksError.InvalidTableFormat;
        }

        return Result<IPacksService.PacksError>.Success();
    }

    private async Task<PackForTable> ConvertPackToTableFormat(Pack pack)
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

        return new PackForTable
        {
            Id = pack.Id,
            Name = pack.Name,
            Rounds = rounds
        };
    }

    private static RoundForTable ConvertRoundToEdit(Round round, Dictionary<Guid, string> correctAnswers)
    {
        var categories = round.QuestionIds.Keys.ToList();

        var costs = categories.Count == 0
            ? []
            : round.QuestionIds[categories[0]].Keys
                .OrderBy(cost => cost)
                .ToList();

        return new RoundForTable
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

    private static QuestionTableCell GetQuestionEditCell(
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

        return new QuestionTableCell
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
