namespace Renligou.Core.Domain.AuthorizationContext.Value;

/// <summary>
/// 部门状态: ACTIVE=启用, INACTIVE=禁用
/// </summary>
public sealed record DepartmentStatus
{
    public static readonly DepartmentStatus Active = new DepartmentStatus("ACTIVE", "启用");
    public static readonly DepartmentStatus Inactive = new DepartmentStatus("INACTIVE", "禁用");

    public string Code { get; }
    public string Description { get; }

    private DepartmentStatus(string code, string description)
    {
        Code = code;
        Description = description;
    }

    public static DepartmentStatus FromCode(string code) => code.ToUpper() switch
    {
        "ACTIVE" => Active,
        "INACTIVE" => Inactive,
        _ => throw new ArgumentException($"无效的部门状态代码: {code}")
    };

    public override string ToString() => Code;
}
