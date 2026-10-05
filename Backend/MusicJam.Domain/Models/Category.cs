namespace MusicJam.Domain.Models;

public class Category
{
    public Guid Id { get; set; }
    public int Order;
    public string CategoryName;
}