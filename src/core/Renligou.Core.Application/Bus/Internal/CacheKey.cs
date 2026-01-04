/// <summary>
/// 内部高性能缓存键。
/// </summary>
namespace Renligou.Core.Application.Bus.Internal;

/// <summary>
/// 专门用于缓存派发委托的键，避免装箱并提供高性能哈希与相等比较。
/// </summary>
internal readonly struct CacheKey : IEquatable<CacheKey>
{
    public readonly Type RequestType;
    public readonly Type ResponseType;

    public CacheKey(Type requestType, Type responseType)
    {
        RequestType = requestType;
        ResponseType = responseType;
    }

    public bool Equals(CacheKey other)
    {
        return RequestType == other.RequestType && ResponseType == other.ResponseType;
    }

    public override bool Equals(object? obj)
    {
        return obj is CacheKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        // 使用针对类型的快速组合哈希
        return HashCode.Combine(RequestType, ResponseType);
    }

    public static bool operator ==(CacheKey left, CacheKey right) => left.Equals(right);
    public static bool operator !=(CacheKey left, CacheKey right) => !left.Equals(right);
}
