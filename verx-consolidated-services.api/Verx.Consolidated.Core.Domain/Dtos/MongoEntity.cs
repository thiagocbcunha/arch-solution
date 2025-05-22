using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Verx.Consolidated.Domain.Dtos;

public class MongoEntity
{
    [BsonId]
    public Guid Id { get; set; }
}
