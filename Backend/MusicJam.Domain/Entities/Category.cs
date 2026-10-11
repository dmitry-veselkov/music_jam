namespace MusicJam.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public int CategoryOrder { get; set; }
    public string CategoryName { get; set; }
}