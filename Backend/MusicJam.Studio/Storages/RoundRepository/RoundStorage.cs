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

    public async Task<Round> Save(Guid packId, Round round)
    {
        var entity = new DbRound
        {
            Title = round.Name,
            
            Questions = await (round,)
        };
 
        db.PackRounds.Add(new PackRound
        {
            PackId = packId,
            Round = entity,
            Order = order
        });
        await db.SaveChangesAsync(ct);
 
        round.Id = entity.Id;
        return round;
    }

    public Task<Round> Delete(Round round)
    {
        throw new NotImplementedException();
    }

    public Task<Round> Update(Round round)
    {
        throw new NotImplementedException();
    }
    
    private async 
}