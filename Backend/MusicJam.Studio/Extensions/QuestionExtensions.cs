using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Tracks;

namespace MusicJam.Studio.Extensions;

public static class QuestionExtensions
{
    public static Question ToModel(this Infrustructure.Entities.Question e) => new()
    {
        Id = e.Id,
        Requested = e.Requested,
        CorrectAnswer = e.CorrectAnswer,
        AnswerTrack = new Track { Id = e.AnswerTrack.Id },
        QuestionTrack = new Track { Id = e.QuestionTrack.Id },
    };
}
