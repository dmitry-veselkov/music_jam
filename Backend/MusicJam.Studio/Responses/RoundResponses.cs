using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Responses;

public class RoundResponse
{
    [Required]
    public string Name { get; set; }
    public List<QuestionResponse> Questions { get; set; } = [];
}

public class QuestionResponse
{
    public Guid? QuestionId { get; set; }
    public string Category { get; set; }
    public int Cost { get; set; }
}