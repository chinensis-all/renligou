namespace Renligou.Core.Shared.Exceptions
{
    public class DomainException : BusinessException
    {
        protected DomainException(string message, string? errorCode = null)
        : base(message, errorCode)
        {
        }
    }
}
