namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Interface for event store operations (MongoDB audit logging).
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// Appends a domain event to the event store.
    /// </summary>
    /// <param name="eventType">The type of event (e.g., "SaleCreated").</param>
    /// <param name="aggregateId">The ID of the aggregate that raised the event.</param>
    /// <param name="aggregateType">The type of aggregate (e.g., "Sale").</param>
    /// <param name="data">The event data to store.</param>
    /// <param name="userId">Optional user ID who triggered the event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AppendAsync(
        string eventType,
        string aggregateId,
        string aggregateType,
        object data,
        string? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all events for a specific aggregate.
    /// </summary>
    /// <param name="aggregateId">The aggregate ID to query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of audit log entries.</returns>
    Task<IEnumerable<AuditLogEntry>> GetByAggregateIdAsync(
        string aggregateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets events by type.
    /// </summary>
    /// <param name="eventType">The event type to query.</param>
    /// <param name="limit">Maximum number of events to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of audit log entries.</returns>
    Task<IEnumerable<AuditLogEntry>> GetByEventTypeAsync(
        string eventType,
        int limit = 100,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an audit log entry from the event store.
/// </summary>
public class AuditLogEntry
{
    public string Id { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string AggregateId { get; set; } = null!;
    public string AggregateType { get; set; } = null!;
    public object? Data { get; set; }
    public string? UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? CorrelationId { get; set; }
}
