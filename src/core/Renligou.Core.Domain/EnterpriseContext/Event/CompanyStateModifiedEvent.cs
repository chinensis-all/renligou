using Renligou.Core.Domain.EnterpriseContext.Value;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.EnterpriseContext.Event
{
    public sealed record CompanyStateModifiedEvent : IIntegrationEvent {

        public DateTimeOffset OccurredAt { get; init; }
        
        public long CompanyId { get; init; }

        public CompanyState State { get; init; }
    }
}
