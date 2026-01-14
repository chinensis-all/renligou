namespace Renligou.Core.Shared.Events
{
    public interface IIntegrationEvent
    {
        DateTimeOffset OccurredAt { get; }
    }
}
