using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM.MongoDB;

/// <summary>
/// MongoDB implementation of the event store for audit logging.
/// </summary>
public class MongoEventStore : IEventStore
{
    private readonly IMongoCollection<AuditLogDocument> _collection;
    private readonly ILogger<MongoEventStore> _logger;

    public MongoEventStore(IConfiguration configuration, ILogger<MongoEventStore> logger)
    {
        _logger = logger;

        var connectionString = configuration.GetConnectionString("MongoDB")
            ?? "mongodb://developer:ev%40luAt10n@localhost:27017/developer_evaluation?authSource=admin";

        try
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("developer_evaluation");
            _collection = database.GetCollection<AuditLogDocument>("audit_logs");
                        
            CreateIndexesAsync().GetAwaiter().GetResult();

            _logger.LogInformation("MongoDB event store initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MongoDB event store");
            throw;
        }
    }

    private async Task CreateIndexesAsync()
    {
        var indexKeys = Builders<AuditLogDocument>.IndexKeys;

        var indexes = new[]
        {
            new CreateIndexModel<AuditLogDocument>(indexKeys.Ascending(x => x.AggregateId)),
            new CreateIndexModel<AuditLogDocument>(indexKeys.Ascending(x => x.EventType)),
            new CreateIndexModel<AuditLogDocument>(indexKeys.Descending(x => x.Timestamp))
        };

        await _collection.Indexes.CreateManyAsync(indexes);
    }

    public async Task AppendAsync(
        string eventType,
        string aggregateId,
        string aggregateType,
        object data,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var bsonData = BsonDocument.Parse(json);

            var document = new AuditLogDocument
            {
                EventType = eventType,
                AggregateId = aggregateId,
                AggregateType = aggregateType,
                Data = bsonData,
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                CorrelationId = Guid.NewGuid().ToString()
            };

            await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Event {EventType} for {AggregateType}:{AggregateId} persisted to audit log",
                eventType, aggregateType, aggregateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to persist event {EventType} for {AggregateType}:{AggregateId}",
                eventType, aggregateType, aggregateId);
        }
    }

    public async Task<IEnumerable<AuditLogEntry>> GetByAggregateIdAsync(
        string aggregateId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<AuditLogDocument>.Filter.Eq(x => x.AggregateId, aggregateId);
            var sort = Builders<AuditLogDocument>.Sort.Descending(x => x.Timestamp);

            var documents = await _collection
                .Find(filter)
                .Sort(sort)
                .ToListAsync(cancellationToken);

            return documents.Select(MapToEntry).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve events for aggregate {AggregateId}", aggregateId);
            return Enumerable.Empty<AuditLogEntry>();
        }
    }

    public async Task<IEnumerable<AuditLogEntry>> GetByEventTypeAsync(
        string eventType,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<AuditLogDocument>.Filter.Eq(x => x.EventType, eventType);
            var sort = Builders<AuditLogDocument>.Sort.Descending(x => x.Timestamp);

            var documents = await _collection
                .Find(filter)
                .Sort(sort)
                .Limit(limit)
                .ToListAsync(cancellationToken);

            return documents.Select(MapToEntry).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve events of type {EventType}", eventType);
            return Enumerable.Empty<AuditLogEntry>();
        }
    }

    private static AuditLogEntry MapToEntry(AuditLogDocument doc)
    {
        return new AuditLogEntry
        {
            Id = doc.Id,
            EventType = doc.EventType,
            AggregateId = doc.AggregateId,
            AggregateType = doc.AggregateType,
            Data = BsonTypeMapper.MapToDotNetValue(doc.Data),
            UserId = doc.UserId,
            Timestamp = doc.Timestamp,
            CorrelationId = doc.CorrelationId
        };
    }
}
