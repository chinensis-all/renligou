using Renligou.Core.Domain.CommonContext.Value;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.EnterpriseContext.Event
{
    public sealed record CompanyAddressModifiedEvent : IIntegrationEvent
    {
        public DateTimeOffset OccurredAt { get; init; }

        public long CompanyId { get; init; }

        public Address Address { get; init; }
    }
}
