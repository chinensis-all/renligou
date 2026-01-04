using Renligou.Core.Shared.Commanding;

/// <summary>
/// 查询标记接口。
/// </summary>
namespace Renligou.Core.Shared.Querying;

/// <summary>
/// 表示一个查询，必须有返回值。
/// </summary>
/// <typeparam name="TResult">查询结果类型。</typeparam>
public interface IQuery<out TResult> : IBaseRequest
{
}
