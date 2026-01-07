namespace Renligou.Core.Infrastructure.Data.Outbox
{
    public class OutboxRow
    {
        public long Id { get; set; }
        public string Category { get; set; } = default!;
        public string SourceType { get; set; } = default!;
        public string SourceId { get; set; } = default!;
        public string EventType { get; set; } = default!;
        public string Payload { get; set; } = default!;
        public string Status { get; set; } = default!;
        public int RetryCount { get; set; }
        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public long Version { get; set; }
    }
}
