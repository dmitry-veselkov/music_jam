using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Responses;

namespace MusicJam.Studio.Extensions;

public static class RoundExtensions
{
    public static RoundResponse ToResponse(this Round round)
    {
        var field = round.Field ?? [];
        var categories = (round.Categories ?? [])
            .Concat(field.Keys)
            .Distinct()
            .ToList();
        
        var costs = field.Values
            .SelectMany(row => row.Keys)
            .Distinct()
            .Order()
            .ToList();

        return new RoundResponse
        {
            Name = round.Name,
            Categories = categories,
            Costs = costs,
            Questions = categories
                .Select(category => costs
                    .Select(cost =>
                    {
                        Question? question = null;
                        if (field.TryGetValue(category, out var row))
                            row.TryGetValue(cost, out question);
                        return new QuestionResponse
                        {
                            QuestionId = question?.Id,
                            Category = category,
                            Cost = cost,
                            CorrectAnswer = question?.CorrectAnswer,
                        };
                    })
                    .ToList())
                .ToList()
        };
    }
}
