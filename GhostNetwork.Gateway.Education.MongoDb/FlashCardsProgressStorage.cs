namespace GhostNetwork.Gateway.Education.MongoDb;

public class FlashCardsProgressStorage : IFlashCardsProgressManager
{
    private readonly MongoDbContext context;

    public FlashCardsProgressStorage(MongoDbContext context)
    {
        this.context = context;
    }

    public async Task<FlashCardsSetUserProgress> FindSetProgressAsync(FlashCardsSetDetails set, EducationUser user)
    {
        var filter = Builders<FlashCardCurrentProgressEntity>.Filter
            .Eq(f => f.SetId, set.Id)
            & Builders<FlashCardCurrentProgressEntity>.Filter
                .Eq(f => f.UserId, user.Id);

        var sp = await context.Progress.Find(filter).FirstOrDefaultAsync()
                          ?? new FlashCardCurrentProgressEntity
                          {
                              SetId = set.Id,
                              UserId = user.Id,
                              CardsProgress = new Dictionary<string, int>(),
                              Date = DateTimeOffset.UtcNow
                          };

        var fraction = set.Cards
            .Aggregate(0, (acc, card) => acc + sp.CardsProgress.GetValueOrDefault(card.Id, 0)) / (decimal)set.Cards.Count;

        return new FlashCardsSetUserProgress(fraction, set.Cards.ToDictionary(
            card => card.Id,
            card => sp.CardsProgress.GetValueOrDefault(card.Id, 0)));
    }

    public async Task UpdateProgressAsync(FlashCardsSetDetails set, EducationUser user, FlashCardSetTestResult results)
    {
        var now = DateTimeOffset.UtcNow;
        await context.History.InsertOneAsync(new FlashCardTestHistoryEntity
        {
            SetId = set.Id,
            UserId = user.Id,
            Date = now,
            Answers = results.Answers
                .Select(a => new FlashCardTestHistoryAnswerEntity
                {
                    CardId = a.CardId,
                    Answer = a.Answer
                })
                .ToList()
        });

        var currentProgress = await FindSetProgressAsync(set, user);
        foreach (var answer in results.Answers)
        {
            currentProgress.CardsProgress[answer.CardId] = set.ValidateAnswer(answer)
                ? Math.Min(currentProgress.CardsProgress[answer.CardId] + 50, 100)
                : Math.Max(currentProgress.CardsProgress[answer.CardId] - 50, 0);
        }

        var filter = Builders<FlashCardCurrentProgressEntity>.Filter
                         .Eq(f => f.SetId, set.Id)
                     & Builders<FlashCardCurrentProgressEntity>.Filter
                         .Eq(f => f.UserId, user.Id);

        var update = Builders<FlashCardCurrentProgressEntity>.Update
            .Set(s => s.SetId, set.Id)
            .Set(s => s.UserId, user.Id)
            .Set(s => s.CardsProgress, currentProgress.CardsProgress)
            .Set(s => s.Date, DateTimeOffset.UtcNow);

        await context.Progress.UpdateOneAsync(filter, update, new UpdateOptions
        {
            IsUpsert = true
        });
    }
}
