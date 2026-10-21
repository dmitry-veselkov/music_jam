using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Requests;

public class GetQuestionsQuery
{
    [Range(0, int.MaxValue)]
    public int Skip { get; set; }
    [Range(1, 100)]
    public int Take { get; set; } = 20;
}

public class CreateQuestionRequest
{
    [Required]
    public Guid QuestionId { get; set; }
    
    [Required, StringLength(512, MinimumLength = 1)]
    public string Requested { get; set; } = string.Empty;
 
    [Required, StringLength(512, MinimumLength = 1)]
    public string CorrectAnswer { get; set; } = string.Empty;
 
    [Required]
    public string QuestionTrack { get; set; } = null!;
 
    [Required]
    public string AnswerTrack { get; set; } = null!;
}

public class UpdateQuestionRequest
{
    [StringLength(512, MinimumLength = 1)]
    public string? Requested { get; set; }
 
    [StringLength(512, MinimumLength = 1)]
    public string? CorrectAnswer { get; set; }
 
    public string? QuestionTrackFile { get; set; }
 
    public string? AnswerTrackFile { get; set; }
}


public enum PatchMode
{
    Cascade,
    PatchCopy
}