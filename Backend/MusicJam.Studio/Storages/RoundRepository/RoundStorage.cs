using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrustructure.Data.Configurations;
using MusicJam.Studio.Infrustructure.Entities;
using DbRound = MusicJam.Studio.Infrustructure.Entities.Round;
using Question = MusicJam.Studio.Models.Packs.Question;
using Round = MusicJam.Studio.Models.Packs.Round;

namespace MusicJam.Studio.Storages;

public class RoundStorage(AppDbContext db, QuestionStorage questionStorage) : IRoundStorage
{
    public Round ToModel(DbRound e) => new()
    {
        Name = e.Title, 
        Field = TryGetRound(e.Id).Result
    };

    public async Task<List<Round>> TryGetByPackId(Guid packId)
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
                            .ToDictionary(rq => rq.Cost, rq => questionStorage.ToModel(rq.Question)))
            })
            .ToList();
    }

    public async Task<Dictionary<string, Dictionary<int, Question>>> TryGetRound(Guid roundId)
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

            costs[rq.Cost] = questionStorage.ToModel(rq.Question);
        }

        return field;
    }

    public async Task<(Round, Guid)> Save(Guid packId, int order, Round round)
    {
        var categoryIds = await GetOrCreateCategories(round.Field.Keys);
        
        var slots = new List<RoundQuestion>();
        foreach (var (categoryName, costs) in round.Field)
        {
            foreach (var (cost, question) in costs)
            {
                slots.Add(new RoundQuestion
                {
                    CategoryId = categoryIds[categoryName],
                    Cost = cost,
                    QuestionId = question.Id
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
    
    public async Task<Dictionary<string, Guid>> GetOrCreateCategories(IEnumerable<string> names)
    {
        var wanted = names.Distinct().ToList();

        var ids = await db.Categories
            .Where(c => wanted.Contains(c.Name))
            .ToDictionaryAsync(c => c.Name, c => c.Id);

        var nextOrder = (await db.Categories.MaxAsync(c => (int?)c.Order) ?? 0) + 1;
        foreach (var name in wanted.Where(n => !ids.ContainsKey(n)))
        {
            var category = new Category { Name = name, Order = nextOrder++ };
            db.Categories.Add(category);
            ids[name] = category.Id;
        }
        return ids;
    }
    public void RemoveCategoriesByIds(IEnumerable<Guid> ids)
    {
        var wanted = ids.Distinct().ToList();
        db.Categories.RemoveRange(
            db.Categories.Where(x => wanted.Contains(x.Id)));
    }

    public async Task<Round> Delete(Guid roundId, Round round)
    {
        
        foreach (var (categoryName, costs) in round.Field)
        {
            db.RoundQuestions.RemoveRange(
                db.RoundQuestions
                    .Where(x => x.RoundId == roundId));
        }
        
        db.PackRounds.RemoveRange(
            db.PackRounds
                .Where(x => x.RoundId == roundId));
        
        var categoryIds = await db.RoundQuestions
            .Where(x => x.RoundId == roundId)
            .Select(x => x.CategoryId)
            .Distinct()
            .ToListAsync();
        
        RemoveCategoriesByIds(categoryIds);
        await db.SaveChangesAsync();
        return round;
    }
    
    public async Task Delete(Guid roundId)
    {
        var categoryIds = db.RoundQuestions
            .Where(x => x.RoundId == roundId)
            .Select(x => x.CategoryId);
        
        db.RoundQuestions.RemoveRange(
            db.RoundQuestions
                .Where(x => x.RoundId == roundId));
        
        db.PackRounds.RemoveRange(
            db.PackRounds
                .Where(x => x.RoundId == roundId));
        RemoveCategoriesByIds(categoryIds);
        await db.SaveChangesAsync();
    }
}