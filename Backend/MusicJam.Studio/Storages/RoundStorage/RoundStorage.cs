using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Extensions;
using MusicJam.Studio.Infrustructure.Data;
using MusicJam.Studio.Infrustructure.Entities;
using DbRound = MusicJam.Studio.Infrustructure.Entities.Round;
using Question = MusicJam.Studio.Models.Packs.Question;
using Round = MusicJam.Studio.Models.Packs.Round;

namespace MusicJam.Studio.Storages.RoundStorage;

public class RoundStorage(AppDbContext db) : IRoundStorage
{
    public Round ToModel(DbRound e) => new()
    {
        Name = e.Title, 
        Categories = GetCategories(e.Id),
        Field = GetRound(e.Id).Result
    };

    public async Task<List<Round>> GetByPackId(Guid packId)
    {
        var packRounds =  await db.PackRounds
            .Where(x => x.PackId == packId)
            .OrderBy(x => x.Order)
            .Include(x => x.Round).ThenInclude(r => r.Questions).ThenInclude(rq => rq.Category)
            .Include(x => x.Round).ThenInclude(r => r.Questions).ThenInclude(rq => rq.Question)
            .ThenInclude(q => q.QuestionTrack)
            .Include(x => x.Round).ThenInclude(r => r.Questions).ThenInclude(rq => rq.Question)
            .ThenInclude(q => q.AnswerTrack)
            .AsSplitQuery()
            .ToListAsync();
        return packRounds
            .Select(pr => new Round
            {
                Name = pr.Round.Title,
                Field = pr.Round.Questions
                    .GroupBy(rq => rq.CategoryId)
                    .OrderBy(g => g.First().Category.Order)
                    .ToDictionary(
                        g => g.First().Category.Name,
                        g => g.OrderBy(rq => rq.Cost)
                            .ToDictionary(rq => rq.Cost,  rq => rq.Question is null ? null : QuestionExtensions.ToModel(rq.Question)))
            })
            .ToList();
    }
    
    public List<string> GetCategories(Guid roundId)
    {
        var roundQuestions = db.RoundQuestions
            .Where(x => x.RoundId == roundId)
            .Include(x => x.Category)
            .Include(roundQuestion => roundQuestion.Question)
            .OrderBy(x => x.Category.Order)
            .Select(x => x.Category.Name)
            .ToList();
        return roundQuestions;
    }

    public async Task<Dictionary<string, Dictionary<int, Question>>> GetRound(Guid roundId)
    {
        var roundQuestions = await db.RoundQuestions
            .Where(x => x.RoundId == roundId)
            .Include(x => x.Category)
            .Include(x => x.Question).ThenInclude(q => q.QuestionTrack)
            .Include(x => x.Question).ThenInclude(q => q.AnswerTrack)
            .OrderBy(x => x.Category.Order).ThenBy(x => x.Cost)
            .ToListAsync();

        var field = new Dictionary<string, Dictionary<int, Question>>();
        foreach (var rq in roundQuestions)
        {
            var categoryName = rq.Category.Name;
            if (!field.TryGetValue(categoryName, out var costs))
            {
                costs = new Dictionary<int, Question>();
                field[categoryName] = costs;
            }

            costs[rq.Cost] = rq.Question.ToModel();
        }

        return field;
    }

    public async Task<(Round, Guid)> Save(Guid packId, int order, Round round)
    {
        var categoryIds = await GetOrCreateCategories(round.Categories);
        
        var slots = new List<RoundQuestion>();
        foreach (var (categoryName, costs) in round.Field)
        {
            foreach (var (cost, question) in costs)
            {
                slots.Add(new RoundQuestion
                {
                    CategoryId = categoryIds[categoryName],
                    Cost = cost,
                    QuestionId = question is null || question.Id == Guid.Empty
                    ? null
                    : question.Id
                });
            }
        }
        
        var dbRound = new DbRound
        {
            Title = round.Name,
            Questions = slots
        };
        
        db.PackRounds.Add(new PackRound
        {
            PackId = packId,
            Round = dbRound,
            Order = order
        });
        
        await db.SaveChangesAsync();
        return (round, dbRound.Id);
    }

    private async Task<Dictionary<string, Guid>> GetOrCreateCategories(List<string> names)
    {
        var ids = await db.Categories
            .Where(c => names.Contains(c.Name))
            .ToDictionaryAsync(c => c.Name, c => c.Id);

        var nextOrder = (await db.Categories.MaxAsync(c => (int?)c.Order) ?? 0) + 1;
        foreach (var name in names.Where(n => !ids.ContainsKey(n)))
        {
            var category = new Category { Name = name, Order = nextOrder++ };
            db.Categories.Add(category);
            ids[name] = category.Id;
        }
        return ids;
    }

    // Категории общие для всех раундов (ищутся по имени), поэтому удаляем только те,
    // на которые больше не ссылается ни одна ячейка
    private void RemoveUnusedCategories(IEnumerable<Guid> ids)
    {
        var wanted = ids.Distinct().ToList();
        db.Categories.RemoveRange(
            db.Categories.Where(x => wanted.Contains(x.Id)
                                     && !db.RoundQuestions.Any(rq => rq.CategoryId == x.Id)));
    }

    public async Task Delete(Guid roundId)
    {
        var categoryIds = await db.RoundQuestions
            .Where(x => x.RoundId == roundId)
            .Select(x => x.CategoryId)
            .Distinct()
            .ToListAsync();

        db.RoundQuestions.RemoveRange(
            db.RoundQuestions
                .Where(x => x.RoundId == roundId));

        db.PackRounds.RemoveRange(
            db.PackRounds
                .Where(x => x.RoundId == roundId));
        await db.SaveChangesAsync();

        RemoveUnusedCategories(categoryIds);
        await db.SaveChangesAsync();
    }
}