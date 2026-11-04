using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Tracks;
using MusicJam.Studio.Responses.Questions;

namespace MusicJam.Studio.Extensions;

public static class QuestionExtensions
{
    public static Question ToModel(this Infrustructure.Entities.Question e) => new()
    {
        Id = e.Id,
        Requested = e.Requested,
        CorrectAnswer = e.CorrectAnswer,
        AnswerTrack = new Track { Id = e.AnswerTrack.ObjectKey },
        QuestionTrack = new Track { Id = e.QuestionTrack.ObjectKey },
    };

    public static QuestionCardResponse ToCardResponse(this Question question)
    {
        return new QuestionCardResponse
        {
            QuestionId = question.Id,
            Requested = question.Requested,
            CorrectAnswer = question.CorrectAnswer,
        };
    }
}
