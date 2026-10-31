using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Requests.Questions;

public class UpdateQuestionRequest
{
    [StringLength(512, MinimumLength = 1)]
    public string? Requested { get; set; }

    [StringLength(512, MinimumLength = 1)]
    public string? CorrectAnswer { get; set; }

    public string? QuestionTrackFile { get; set; }

    public string? AnswerTrackFile { get; set; }
}
