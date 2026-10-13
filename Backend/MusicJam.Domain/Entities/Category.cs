using MusicJam.Domain.Common;

namespace MusicJam.Domain.Entities;

public class Category : Entity
{
    public int Order { get; set; }
    public string Name { get; set; }
}