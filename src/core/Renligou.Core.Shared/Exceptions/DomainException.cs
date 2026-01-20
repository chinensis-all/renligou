namespace Renligou.Core.Shared.Exceptions
{
    public abstract class DomainException : BusinessException
    {
        protected DomainException(string message) : base(message) { }

        public abstract string ErrorCode { get; } 
    }
}
