using Renligou.Core.Application.Kernel.Commands;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.Kernel.Handlers
{
    /// <summary>
    /// Handles the creation of outbox entries by processing integration events collected during command execution.
    /// </summary>
    /// <remarks>This handler collects integration events from the outbox collector and persists them using
    /// the provided outbox repository. It is intended for internal use within the application's command handling
    /// infrastructure.</remarks>
    public class CreateOutboxHandler : ICommandHandler<CreateOutboxCommand>
    {
        private readonly IOutboxRepository _outboxRepository;

        public CreateOutboxHandler(IOutboxRepository outboxRepository)
        {
            _outboxRepository = outboxRepository;
        }

        /// <summary>
        /// Processes the specified outbox command by persisting all collected integration events to the outbox
        /// repository asynchronously.
        /// </summary>
        /// <remarks>If there are no integration events collected by the outbox collector, this method
        /// completes without performing any repository operations.</remarks>
        /// <param name="command">The command that triggers the outbox handling operation. Must not be null.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleAsync(CreateOutboxCommand command, CancellationToken cancellationToken)
        {
            List<(IIntegrationEvent, string, string, string)> items = OutboxCollector.Drain().ToList();

            if (items.Any())
            {
                foreach (var (@event, category, aggregateType, aggregateId) in items)
                {
                    await _outboxRepository.AddAsync(@event, category, aggregateType, aggregateId);
                }
            }
        }
    }
}
