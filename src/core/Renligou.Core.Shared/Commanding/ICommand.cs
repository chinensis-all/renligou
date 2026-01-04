/// <summary>
/// 命令标记接口。
/// </summary>
namespace Renligou.Core.Shared.Commanding;

/// <summary>
/// 表示一个不带返回值的命令。
/// </summary>
public interface ICommand : IBaseRequest
{
}

/// <summary>
/// 表示一个带返回值的命令。
/// </summary>
/// <typeparam name="TResult">返回结果类型。</typeparam>
public interface ICommand<out TResult> : IBaseRequest
{
}

/// <summary>
/// 通用请求标记接口（内部使用或作为管道层抽象）。
/// </summary>
public interface IBaseRequest
{
}
