using Renligou.Core.Shared.Events;

namespace Renligou.Core.Shared.Ddd
{
    public class AggregateBase
    {
        public AggregateId Id { get; set; }

        /// <summary>
        /// Registers an integration event for processing within the domain event system.
        /// </summary>
        /// <param name="event">The integration event to be registered. Cannot be null.</param>
        public void registerEnent(IIntegrationEvent @event)
        {
            OutboxCollector.Collect(@event, "Domain", this.GetType().Name, this.Id.isNew.ToString());
        }
    }
}
