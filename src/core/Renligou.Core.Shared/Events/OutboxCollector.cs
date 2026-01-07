namespace Renligou.Core.Shared.Events
{
    /// <summary>
    /// Provides a static context-local collector for integration events, allowing events to be accumulated and
    /// retrieved within the current asynchronous flow.
    /// </summary>
    /// <remarks>DomainEventCollector uses an AsyncLocal storage to maintain a list of integration events
    /// scoped to the current asynchronous context. This enables event collection and retrieval that is isolated per
    /// logical execution flow, such as per web request or background task. The collector is intended for scenarios
    /// where events need to be gathered and processed together, such as batching domain events for publishing. All
    /// members are static and thread-safe within the context of AsyncLocal usage.</remarks>
    public class OutboxCollector
    {
        private static readonly AsyncLocal<List<(IIntegrationEvent, string, string, string)>?> _events = new();

        public static void Collect(IIntegrationEvent @event, string category, string aggregateType, string aggregateId)
        {
            var list = _events.Value;
            if (list == null)
            {
                list = new List<(IIntegrationEvent, string, string, string)>();
                _events.Value = list;
            }

            list.Add((@event, category, aggregateType, aggregateId));
        }

        public static IReadOnlyList<(IIntegrationEvent, string, string, string)> Drain()
        {
            try
            {
                return _events.Value != null ? _events.Value.AsReadOnly() : Array.Empty<(IIntegrationEvent, string, string, string)>();
            }
            finally
            {
                _events.Value = null;
            }
        }

        public static void Clear()
        {
            _events.Value = null;
        }
    }
}
