using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.EventingContext.Repo
{
    public interface IOutboxRepository : IRepository
    {
        /// <summary>
        /// Asynchronously adds an integration event to the event store with the specified category and aggregate
        /// information.
        /// </summary>
        /// <param name="event">The integration event to add. Cannot be null.</param>
        /// <param name="category">The category under which the event is classified. Cannot be null or empty.</param>
        /// <param name="aggregateType">The type of the aggregate associated with the event. Cannot be null or empty.</param>
        /// <param name="aggregateId">The unique identifier of the aggregate instance related to the event. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task AddAsync(IIntegrationEvent @event, string category, string aggregateType, string aggregateId);

        /// <summary>
        /// Asynchronously adds a collection of integration events to the event store under the specified category and
        /// aggregate information.
        /// </summary>
        /// <param name="events">The collection of integration events to add. Cannot be null or contain null elements.</param>
        /// <param name="category">The category under which the events are stored. Cannot be null or empty.</param>
        /// <param name="aggregateType">The type of the aggregate associated with the events. Cannot be null or empty.</param>
        /// <param name="aggregateId">The unique identifier of the aggregate to which the events belong. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task AddAsync(IEnumerable<IIntegrationEvent> @events, string category, string aggregateType, string aggregateId);
    }
}
