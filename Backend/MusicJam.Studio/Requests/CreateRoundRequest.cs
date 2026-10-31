using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Requests;

public class CreateRoundRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public List<List<RoundQuestionRequest>> Questions { get; set; } =
    [
        [
            new()
            {
                Cost = 100,
                Category = "Первая",
                QuestionId = null
            }
        ]
    ];

    public IEnumerable<RoundQuestionRequest> Cells => Questions.SelectMany(row => row);
}
