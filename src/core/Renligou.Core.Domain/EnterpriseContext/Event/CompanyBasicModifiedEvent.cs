using Renligou.Core.Domain.EnterpriseContext.Value;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.EnterpriseContext.Event
{
    public sealed record CompanyBasicModifiedEvent : IIntegrationEvent
    {
        public DateTimeOffset OccurredAt { get; init; }

        public long CompanyId { get; init; }

        public CompanyType CompanyType { get; init; }

        public string CompanyCode { get; init; }

        public string CompanyName { get; init; }

        public string CompanyShortName { get; init; }

        public string LegalPersonName { get; init; }

        public string CreditCode { get; init; }

        public string RegisteredAddress { get; init; }

        public string Remark { get; init; }
    }
}
