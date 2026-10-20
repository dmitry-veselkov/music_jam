using MusicJam.Studio.Models.Packs;
using Question = MusicJam.Studio.Models.Packs.Question;


namespace MusicJam.Studio.Storages;

public interface IRoundStorage
{
    Task<List<Round>> TryGetByPackId(Guid packId);
    
    Task<Dictionary<string, Dictionary<int, Question>>> TryGetRound(Guid roundId);


    Task<Round> Save(Guid packId, Round round);
    Task<Round> Delete(Round round);
    Task<Round> Update(Round round);
}