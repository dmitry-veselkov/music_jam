using MusicJam.Studio.Models.Packs;


namespace MusicJam.Studio.Storages.RoundRepository;

public interface IRoundStorage
{
    Task<Round?> TryGetUserByEmail(string email);

    Task<Round> SaveRound(Round round);
}