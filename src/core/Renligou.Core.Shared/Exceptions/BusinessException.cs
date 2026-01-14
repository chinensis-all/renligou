namespace Renligou.Core.Shared.Exceptions
{
    public abstract class BusinessException : Exception
    {
        protected BusinessException(
        string message,
        string? errorCode = null,
        Exception? innerException = null)
        : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public string? ErrorCode { get; }
    }
}
