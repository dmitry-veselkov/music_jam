using MusicJam.Studio.Infrustructure.Entities;

namespace MusicJam.Studio.Infrustructure.Data.Configurations.Services;

public class CategoryHandler
{
    private readonly AppDbContext _dataContext;
    
    public CategoryHandler(AppDbContext dataContext)
    {
        _dataContext = dataContext;
    }

    public Category? GetCategoryById(Guid id)
    {
        return _dataContext.Categories.Find(id);
    }
    
    public List<Category> GetAllCategoriesPerRound(Round round)
    {
        return _dataContext.RoundQuestions
            .Where(x => x.RoundId == round.Id)
            .Select(x => x.Category)
            .ToList();
    }
    
    public async Task AddNewCategory(Category category)
    {
        _dataContext.Categories.Add(category);
        await _dataContext.SaveChangesAsync();
    }
    
    public async Task RemoveCategory(Category category)
    {
        _dataContext.Categories.Remove(category);
        await _dataContext.SaveChangesAsync();
    }
    
}