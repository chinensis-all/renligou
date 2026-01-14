namespace Renligou.Core.Shared.Exceptions
{
    public class AppException : BusinessException
    {
        protected AppException(string message, string? errorCode = null)
       : base(message, errorCode)
        {
        }
    }
}
