/// <summary>
/// 命令查询总线异常体系。
/// </summary>
namespace Renligou.Core.Shared.Exceptions;

/// <summary>
/// 总线底层基类异常。
/// </summary>
public class CommandQueryBusException : Exception
{
    public CommandQueryBusException(string message) : base(message) { }
    public CommandQueryBusException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// 未找到处理程序时抛出的异常。
/// </summary>
public class HandlerNotFoundException : CommandQueryBusException
{
    public HandlerNotFoundException(Type requestType) 
        : base($"未找到请求类型 {requestType.Name} 的处理程序。") { }
}

/// <summary>
/// 找到多个处理程序（预期仅一个）时抛出的异常。
/// </summary>
public class MultipleHandlersFoundException : CommandQueryBusException
{
    public MultipleHandlersFoundException(Type requestType) 
        : base($"为请求类型 {requestType.Name} 找到了多个处理程序，但预期只应有一个。") { }
}

/// <summary>
/// 处理程序注册无效时抛出的异常。
/// </summary>
public class InvalidHandlerRegistrationException : CommandQueryBusException
{
    public InvalidHandlerRegistrationException(string message) : base(message) { }
}
