using MusicJam.Studio.Infrustructure.Entities;

namespace MusicJam.Studio.Infrustructure.Data.Configurations.Services;

public class QuestionHandler
{
    private readonly AppDbContext _dataContext;
    
    public QuestionHandler(AppDbContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public List<Question> GetAllUserQuestions (Guid userId)
    {
        return _dataContext.Questions
            .Where(x => x.UserId == userId)
            .ToList();
    }
    
    public Question GetUserQuestion(Guid questionId)
    {
        return _dataContext.Questions.First(x => x.Id == questionId);

    }
    
    public async Task<Guid> AddQuestion(Question question)
    {
        _dataContext.Questions.Add(question);
        await _dataContext.SaveChangesAsync();
        return question.Id;
    }
    
    public async Task<Guid> AddQuestionWithPack(Question question, Pack pack)
    {
        _dataContext.Questions.Add(question);
        _dataContext.QuestionUsages.Add(new QuestionUsages()
        {
            Pack = pack,
            PackId = pack.Id,
            Question = question,
            QuestionId = question.Id
        });
        await _dataContext.SaveChangesAsync();
        return question.Id;
    }
    
    public async Task DeleteQuestion(Question question)
    {
        _dataContext.Questions.Remove(question);
        _dataContext.QuestionUsages.RemoveRange(_dataContext.QuestionUsages
                .Where(x => x.Question == question));
        await _dataContext.SaveChangesAsync();
    }
}