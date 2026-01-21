using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Renligou.Core.Infrastructure.Persistence.Pos;

/// <summary>
/// 部门持久化对象
/// </summary>
[Table("departments")]
public class DepartmentPo
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("parent_id")]
    public long ParentId { get; set; }

    [Column("company_id")]
    public long CompanyId { get; set; }

    [Column("dept_name")]
    [Required]
    [MaxLength(100)]
    public string DeptName { get; set; } = string.Empty;

    [Column("dept_code")]
    [Required]
    [MaxLength(30)]
    public string DeptCode { get; set; } = string.Empty;

    [Column("description")]
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Column("sorter")]
    public int Sorter { get; set; }

    [Column("status")]
    [Required]
    public string Status { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
