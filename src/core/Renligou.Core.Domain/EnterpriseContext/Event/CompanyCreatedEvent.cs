using Renligou.Core.Domain.CommonContext.Value;
using Renligou.Core.Domain.EnterpriseContext.Value;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.EnterpriseContext.Event
{
    public sealed record CompanyCreatedEvent : IIntegrationEvent {
        public DateTimeOffset OccurredAt { get; init; }

        public long CompanyId { get; init; }

        public CompanyType CompanyType { get; init; }

        public string CompanyCode { get; init; } = string.Empty;

        public string CompanyName { get; init; } = string.Empty;

        public string CompanyShortName { get; init; } = string.Empty;

        public string LegalPersonName { get; init; } = string.Empty;

        public string CreditCode { get; init; } = string.Empty;

        public string RegisteredAddress { get; init; } = string.Empty;

        public string Remark { get; init; } = string.Empty;

        public Address Address { get; init; } = default!;

        public CompanyState State { get; init; } = default!;
    } 
}
