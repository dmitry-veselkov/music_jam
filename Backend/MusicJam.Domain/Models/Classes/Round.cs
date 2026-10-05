namespace MusicJam.Domain;

public class Round
{
    private int order;
    private Dictionary<(Category, Cost), Question> questions;
}