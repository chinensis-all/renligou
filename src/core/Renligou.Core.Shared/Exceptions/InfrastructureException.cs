namespace Renligou.Core.Shared.Exceptions
{
    public class InfrastructureException : BusinessException
    {
        protected InfrastructureException(
        string message,
        Exception? inner = null)
        : base(message, "infrastructure_error", inner)
        {
        }
    }
}
