namespace GhostNetwork.Gateway.Education.MongoDb;

public class FlashCardTestHistoryEntity
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("setId")]
    public string SetId { get; set; } = null!;

    [BsonElement("userId")]
    public string UserId { get; set; } = null!;

    [BsonElement("date")]
    public DateTimeOffset Date { get; set; }

    [BsonElement("answers")]
    public List<FlashCardTestHistoryAnswerEntity> Answers { get; set; }
}