using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Responses;

public class RoundResponse
{
    [Required]
    public string Name { get; set; }
    public List<string> Categories { get; set; } = [];
    public List<int> Costs { get; set; } = [];
    public List<List<QuestionResponse>> Questions { get; set; } = [];
}

public class QuestionResponse
{
    public Guid? QuestionId { get; set; }
    public string Category { get; set; }
    public int Cost { get; set; }
    public string? CorrectAnswer { get; set; }
}
