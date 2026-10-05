namespace MusicJam.Domain.Models;

public class Round
{
    public Guid Id { get; set; }
    public int Order;
    public Dictionary<(Category, Cost), Question> Questions;
}