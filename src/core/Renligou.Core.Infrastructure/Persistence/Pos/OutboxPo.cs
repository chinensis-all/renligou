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

        public DateTime OccurredAt { get; set; }

        public DateTime SentAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long Version { get; set; } = 0;
    }
}
