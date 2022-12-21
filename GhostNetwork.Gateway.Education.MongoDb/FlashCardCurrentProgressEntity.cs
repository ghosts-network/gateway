namespace GhostNetwork.Gateway.Education.MongoDb;

public class FlashCardCurrentProgressEntity
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("setId")]
    public string SetId { get; set; } = null!;

    [BsonElement("userId")]
    public string UserId { get; set; } = null!;

    [BsonElement("cardsProgress")]
    public Dictionary<string, int> CardsProgress { get; set; }

    [BsonElement("date")]
    public DateTimeOffset Date { get; set; }
}
