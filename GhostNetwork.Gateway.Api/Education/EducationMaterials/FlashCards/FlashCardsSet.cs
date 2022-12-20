#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Gateway.Api.Education.EducationMaterials.FlashCards;

public record FlashCardsSet(string Id, string Title, FlashCardsSetInfo Details);

public record FlashCardsSetInfo(int TotalCards);

public record FlashCardsSetUserProgress(decimal Fraction, Dictionary<string, int> CardsProgress)
{
    public static FlashCardsSetUserProgress Empty => new FlashCardsSetUserProgress(decimal.Zero, new Dictionary<string, int>());
}

public record FlashCardsSetDetails(
    string Id,
    string Title,
    IReadOnlyCollection<FlashCard> Cards,
    FlashCardsSetUserProgress? Progress)
{
    public bool ValidateAnswer(FlashCardTestAnswer answer)
    {
        return Cards.First(c => c.Id == answer.CardId).Definition == answer.Answer;
    }
}

public record FlashCard(string Definition, string Description, IEnumerable<string> Examples)
{
    public string Id => Definition;
}

public record FlashCardSetTestResult(IEnumerable<FlashCardTestAnswer> Answers);

public record EducationUser(string Id);

public record FlashCardTestAnswer(string CardId, string Answer);
