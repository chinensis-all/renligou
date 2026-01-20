using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Renligou.Core.Infrastructure.Persistence.Pos;

[Table("departments")]
[Index(nameof(CompanyId), nameof(ParentId), nameof(DeptName), IsUnique = true)]
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
    [MaxLength(30)]
    public string DeptCode { get; set; } = string.Empty;

    [Column("description")]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Column("sorter")]
    public int Sorter { get; set; }

    [Column("status")]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string Status { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
