using Renligou.Core.Domain.EnterpriseContext.Value;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.Enterprise.Commands
{
    public sealed record CreateCompanyCommand : ICommand<Result>
    {
        public string CompanyType { get; init; }

        public string CompanyCode { get; init; }

        public string CompanyName { get; init; }

        public string CompanyShortName { get; init; }

        public string LegalPersonName { get; init; }

        public string CreditCode { get; init; }

        public string RegisteredAddress { get; init; }

        public string Remark { get; init; }
        public long ProvinceId { get; init; }

        public long CityId { get; init; }

        public long DistrictId { get; init; }

        public string CompletedAddress { get; init; }

        public bool Enabled { get; init; }

        public DateOnly? EffectiveDate { get; init; }

        public DateOnly? ExpiredDate { get; init; }

        public Result Validate()
        {
            if (!Enum.TryParse<CompanyType>(CompanyType, ignoreCase: true, out _))
            {
                return Result.Fail("Company.Create.Error", $"非法的公司类型: {CompanyType}");
            }

            if (string.IsNullOrEmpty(CompanyName))
            {
                return Result.Fail("Company.Create.Error", "缺失公司名称");
            }

            if (string.IsNullOrEmpty(CompanyShortName))
            {
                return Result.Fail("Company.Create.Error", "缺失公司简称");
            }

            if (ProvinceId <= 0 || CityId <= 0 || DistrictId <= 0)
            {
                return Result.Fail("Company.Create.Error", "缺失公司地址信息");
            }

            if (Enabled && EffectiveDate == null)
            {
                return Result.Fail("Company.Create.Error", "启用状态下缺失生效日期");
            }

            if (Enabled && EffectiveDate == null)
            {
                return Result.Fail("Company.Create.Error", "启用状态下缺失失效日期");
            }

            return Result.Ok();
        }
    }
}
