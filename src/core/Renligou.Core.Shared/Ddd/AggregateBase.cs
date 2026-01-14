using Renligou.Core.Shared.Events;

namespace Renligou.Core.Shared.Ddd
{
    public class AggregateBase
    {
        public AggregateId Id { get; set; }

        private List<IIntegrationEvent> _events = new();

        /// <summary>
        /// Registers an integration event for processing within the domain event system.
        /// </summary>
        /// <param name="event">The integration event to be registered. Cannot be null.</param>
        public void RegisterEvent(IIntegrationEvent @event)
        {
            OutboxCollector.Collect(@event, "Domain", this.GetType().Name, this.Id.id.ToString());
            _events.Add(@event);
        }

        /// <summary>
        /// Gets a read-only list of all registered integration events.
        /// </summary>
        /// <returns>A read-only list containing the registered integration events. The list may be empty if no events have been
        /// registered. Elements in the list may be null.</returns>
        public IReadOnlyList<IIntegrationEvent> GetRegisteredEvents()
        {
            return _events.AsReadOnly();
        }

        /// <summary>
        /// Removes all registered events from the collection.
        /// </summary>
        /// <remarks>After calling this method, the collection of registered events will be empty. This
        /// operation cannot be undone.</remarks>
        public void ClearRegisteredEvents()
        {
            _events.Clear();
        }
    }
}
