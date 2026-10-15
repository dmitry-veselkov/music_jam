namespace MusicJam.Studio.Storages.QuestionsStorage;

internal class QuestionsStorage : IQuestionsStorage
{
    public async Task<Dictionary<Guid, string>> GetCorrectAnswers(List<Guid> questionIds)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
