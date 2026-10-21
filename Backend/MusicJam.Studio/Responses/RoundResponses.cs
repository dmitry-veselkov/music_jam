using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Responses;

public class RoundResponse
{
    [Required]
    public string Name { get; set; }

    [Required]
    public List<string> Categories { get; set; } = [];

    [Required]
    public List<int> Costs { get; set; } = [];

    public List<List<QuestionResponse>> Questions { get; set; } = [];
}

public class QuestionResponse
{
    [Required]
    public Guid QuestionId { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public int Cost { get; set; }
    public string CorrectAnswer { get; set;}
    public string Requested { get; set;}

}