namespace Renligou.Api.Boss.Requests;

/// <summary>
/// 修改菜单可见性请求
/// </summary>
public record ChangeMenuVisibilityRequest
{
    /// <summary>
    /// 是否隐藏
    /// </summary>
    public bool IsHidden { get; init; }
}
