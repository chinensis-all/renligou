using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Renligou.Core.Infrastructure.Persistence.Pos
{
    /// <summary>
    /// Po: 可靠事件存储表
    /// </summary>
    [Table("_events")]
    public class OutboxPo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Category { get; set; } = string.Empty;

        public string SourceType { get; set; } = string.Empty;

        public string SourceId { get; set; } = string.Empty;

        public string EventType { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public int RetryCount { get; set; } = 0;

        public DateTimeOffset OccurredAt { get; set; }

        public DateTimeOffset SentAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public long Version { get; set; } = 0;
    }
}
