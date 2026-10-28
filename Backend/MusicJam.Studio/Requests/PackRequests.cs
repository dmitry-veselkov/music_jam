using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MusicJam.Studio.Models.Packs;

namespace MusicJam.Studio.Requests;

public class GetPackRequest
{
    [Range(0, int.MaxValue)]
    public int Skip { get; set; }
    [Range(1, 100)]
    public int Take { get; set; } = 20;
}

public class CreatePackRequest
{
    [Required] public Guid PackId;
    public string Name { get; set; } = string.Empty;

    [Required] public List<CreateRoundRequest> Rounds { get; set; } = [];
}

public class UpdatePackRequest : CreatePackRequest;

public class CreateRoundRequest
{
    [Required] public string Name { get; set; } = string.Empty;
    
    [Required]
    public List<RoundQuestionRequest> Questions { get; set; } = [new ()
    {
        Cost = 100,
        Category = "Первая",
        QuestionId = Guid.Empty
    }];
}

public class RoundQuestionRequest
{
    public int Cost { get; set; }
    public string Category { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }
}

