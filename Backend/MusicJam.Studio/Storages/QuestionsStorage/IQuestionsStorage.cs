namespace MusicJam.Studio.Storages.QuestionsStorage;

public interface IQuestionsStorage
{
    Task<Dictionary<Guid, string>> GetCorrectAnswers(List<Guid> questionIds);
}
