namespace GhostNetwork.Gateway.Education.MongoDb;

public class FlashCardTestHistoryAnswerEntity
{
    [BsonElement("cardId")]
    public string CardId { get; set; }

    [BsonElement("answer")]
    public string Answer { get; set; }
}