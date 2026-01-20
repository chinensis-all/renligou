using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Renligou.Core.Infrastructure.Persistence.Pos
{
    /// <summary>
    /// 权限持久化对象
    /// </summary>
    [Table("permissions")]
    public class PermissionPo
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("group_id")]
        [Required]
        public long GroupId { get; set; }

        [Column("permission_name")]
        [Required]
        [MaxLength(100)]
        public string PermissionName { get; set; } = string.Empty;

        [Column("display_name")]
        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [Column("description")]
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("deleted_at")]
        public long DeletedAt { get; set; }
    }
}
