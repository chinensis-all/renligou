using System.ComponentModel;

namespace Renligou.Core.Application.Enterprise.Queries
{
    public sealed record CompanyDetailDto
    {
        [Description("公司ID")]
        public string CompanyId { get; init; }

        public string CompanyCode { get; init; }

        public string CompanyType { get; init; }

        public string CompanyName { get; init; }

        public string CompanyShortName { get; init; }

        public string LegalPersonName { get; init; }

        public string CreditCode { get; init; }

        public string RegisteredAddress { get; init; }

        public string ProvinceId { get; init; }

        public string Province { get; init; }

        public string CityId { get; init; }

        public string City { get; init; }

        public string DistrictId { get; init; }

        public string District { get; init; }

        public string CompletedAddress { get; init; }

        public bool Enabled { get; init; }

        public string EffectiveDate { get; init; }

        public string ExpiredDate { get; init; }

        public string Remark { get; init; }

        public string CreatedAt { get; init; }

        public string UpdatedAt { get; init; }
    }
}
