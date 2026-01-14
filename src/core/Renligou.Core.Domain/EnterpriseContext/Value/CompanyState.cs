namespace Renligou.Core.Domain.EnterpriseContext.Value
{
    public readonly record struct CompanyState(
       bool Enabled,
       DateOnly? EffectiveDate,
       DateOnly? ExpiredDate
    ) {
        public CompanyState Enable()
            => this with { Enabled = true };

        public CompanyState Disable()
            => this with { Enabled = false };

        public CompanyState ChangeEffectivePeriod(
            DateOnly? effectiveDate,
            DateOnly? expiredDate)
            => this with
            {
                EffectiveDate = effectiveDate,
                ExpiredDate = expiredDate
            };
    }
}
