using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Renligou.Core.Infrastructure.Persistence.Pos;

/// <summary>
/// 菜单持久化对象
/// </summary>
[Table("menus")]
public class MenuPo
{
    [Key]
    public long Id { get; set; }

    public long ParentId { get; set; }

    [Required]
    [MaxLength(100)]
    public string MenuName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string MenuTag { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Path { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Component { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Icon { get; set; } = string.Empty;

    public int Sorter { get; set; }

    public bool IsHidden { get; set; }

    [MaxLength(255)]
    public string PermitButtons { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public long DeletedAt { get; set; }
}
