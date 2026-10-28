using MusicJam.Studio.Models.Packs;
using Question = MusicJam.Studio.Models.Packs.Question;


namespace MusicJam.Studio.Storages;

public interface IRoundStorage
{
    Task<List<Round>> TryGetByPackId(Guid packId);
    
    Task<Dictionary<string, Dictionary<int, Question>>> TryGetRound(Guid roundId);


    Task<(Round, Guid)> Save(Guid packId, int order, Round round);
    
    Task Delete(Guid roundId);
    
}