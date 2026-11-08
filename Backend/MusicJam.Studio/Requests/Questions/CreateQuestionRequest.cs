using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Requests.Questions;

public class CreateQuestionRequest
{
    [Required, StringLength(512, MinimumLength = 1)]
    public string Requested { get; set; } = string.Empty;

    [Required, StringLength(512, MinimumLength = 1)]
    public string CorrectAnswer { get; set; } = string.Empty;

    public IFormFile QuestionTrack { get; set; }

    public IFormFile AnswerTrack { get; set; }
}
