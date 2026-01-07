namespace Renligou.Core.Domain.EventingContext.Value
{
    public enum OutboxStatus
    {
        NEW,
        SENDING,
        SENT,
        FAILED
    }
}
