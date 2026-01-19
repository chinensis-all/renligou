using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Renligou.Core.Infrastructure.Persistence.Pos
{
    [Table("permission_groups")]
    public class PermissionGroupPo
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string GroupName { get; set; } = string.Empty;

        [Required]
        public string DisplayName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Column("parent_id")]
        public long ParentId { get; set; }

        public int Sorter { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long DeletedAt { get; set; }
    }
}
