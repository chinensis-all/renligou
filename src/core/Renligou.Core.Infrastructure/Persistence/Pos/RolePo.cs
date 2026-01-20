using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Renligou.Core.Infrastructure.Persistence.Pos;

/// <summary>
/// 角色持久化对象
/// </summary>
[Table("roles")]
public class RolePo
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("role_name")]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色显示名称 (角色权限字符串)
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 删除时间(逻辑删除)
    /// </summary>
    [Required]
    [Column("deleted_at")]
    public long DeletedAt { get; set; } = 0;
}
