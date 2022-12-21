namespace GhostNetwork.Gateway.Education.MongoDb;

public class MongoDbContext
{
    private readonly IMongoDatabase database;

    static MongoDbContext()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
    }

    public MongoDbContext(IMongoDatabase database)
    {
        this.database = database;
    }

    public IMongoCollection<FlashCardTestHistoryEntity> History =>
        database.GetCollection<FlashCardTestHistoryEntity>("edu_flashCardTestHistory");

    public IMongoCollection<FlashCardCurrentProgressEntity> Progress =>
        database.GetCollection<FlashCardCurrentProgressEntity>("edu_flashCardCurrentProgress");
}