using Renligou.Core.Domain.CommonContext.Value;
using Renligou.Core.Domain.EnterpriseContext.Event;
using Renligou.Core.Domain.EnterpriseContext.Value;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.EnterpriseContext.Model
{
    public class Company : AggregateBase
    {
        public CompanyType CompanyType { get; private set; }
        public string CompanyCode { get; private set; } = default!;
        public string CompanyName { get; private set; } = default!;
        public string CompanyShortName { get; private set; } = string.Empty;

        public string LegalPersonName { get; private set; } = string.Empty;
        public string CreditCode { get; private set; } = string.Empty;
        public string RegisteredAddress { get; private set; } = string.Empty;

        public Address Address { get; private set; }
        public CompanyState State { get; private set; }

        public string Remark { get; private set; } = string.Empty;

        protected Company() { }

        public Company(
            AggregateId id,
            CompanyType companyType,
            string companyCode,
            string companyName,
            string companyShortName,
            string legalPersonName,
            string creditCode,
            string registeredAddress,
            string remark,
            Address address,
            CompanyState state
        )
        {
            Id = id;
            CompanyType = companyType;
            CompanyCode = companyCode;
            CompanyName = companyName;
            CompanyShortName = companyShortName;
            LegalPersonName = legalPersonName;
            CreditCode = creditCode;
            RegisteredAddress = registeredAddress;
            Remark = remark;
            Address = address;
            State = state;
        }

        public void Create()
        {
            CompanyCreatedEvent @event = new CompanyCreatedEvent
            {
                CompanyId = Id.id,
                CompanyType = CompanyType,
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                CompanyShortName = CompanyShortName,
                LegalPersonName = LegalPersonName,
                CreditCode = CreditCode,
                RegisteredAddress = RegisteredAddress,
                Remark = Remark,
                Address = Address,
                State = State,
                OccurredAt = DateTimeOffset.UtcNow
            };

            this.RegisterEvent(@event);
        }

        public void ModifyBasic(
            CompanyType companyType,
            string companyCode,
            string companyName,
            string companyShortName,
            string legalPersonName,
            string creditCode,
            string registeredAddress,
            string remark
        )
        {
            CompanyType = companyType;
            CompanyCode = companyCode;
            CompanyName = companyName;
            CompanyShortName = companyShortName;
            LegalPersonName = legalPersonName;
            CreditCode = creditCode;
            RegisteredAddress = registeredAddress;
            Remark = remark;

            var @event = new CompanyBasicModifiedEvent
            {
                CompanyId = Id.id,
                CompanyType = companyType,
                CompanyCode = companyCode,
                CompanyName = companyName,
                CompanyShortName = companyShortName,
                LegalPersonName = legalPersonName,
                CreditCode = creditCode,
                RegisteredAddress = registeredAddress,
                Remark = remark,
                OccurredAt = DateTimeOffset.UtcNow
            };
            this.RegisterEvent(@event);
        }

        public void ModifyAddress(Address address)
        {
            Address = address;
            var @event = new CompanyAddressModifiedEvent
            {
                CompanyId = Id.id,
                Address = address,
                OccurredAt = DateTimeOffset.UtcNow
            };
            this.RegisterEvent(@event);
        }

        public void ModifyState(CompanyState state)
        {
            State = state;
            RegisterCompanyStateModifiedEvent();
        }

        public void Enable()
        {
            State = State.Enable();
            RegisterCompanyStateModifiedEvent();
        }

        public void Disable()
        {
            State = State.Disable();
            RegisterCompanyStateModifiedEvent();
        }

        private void RegisterCompanyStateModifiedEvent()
        {
            var @event = new CompanyStateModifiedEvent
            {
                CompanyId = Id.id,
                State = State,
                OccurredAt = DateTimeOffset.UtcNow
            };
            this.RegisterEvent(@event);
        }
    }
}
