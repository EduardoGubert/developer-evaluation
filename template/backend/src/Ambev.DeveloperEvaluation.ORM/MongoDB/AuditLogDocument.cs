using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ambev.DeveloperEvaluation.ORM.MongoDB;

/// <summary>
/// MongoDB document model for audit log entries.
/// </summary>
public class AuditLogDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("eventType")]
    public string EventType { get; set; } = null!;

    [BsonElement("aggregateId")]
    public string AggregateId { get; set; } = null!;

    [BsonElement("aggregateType")]
    public string AggregateType { get; set; } = null!;

    [BsonElement("data")]
    public BsonDocument Data { get; set; } = null!;

    [BsonElement("userId")]
    public string? UserId { get; set; }

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }

    [BsonElement("correlationId")]
    public string? CorrelationId { get; set; }
}
