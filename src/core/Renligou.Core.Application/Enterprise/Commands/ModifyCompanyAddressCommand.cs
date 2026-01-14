using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.Enterprise.Commands
{
    public sealed record ModifyCompanyAddressCommand : ICommand<Result>
    {
        public long CompanyId { get; init; } 

        public long ProvinceId { get; init; }

        public long CityId { get; init; }

        public long DistrictId { get; init; }

        public string CompletedAddress { get; init; } = string.Empty;

        public Result Validate()
        {
            if (CompanyId <= 0)
            {
                return Result.Fail("Company.Address.Modify.Error", "未选择公司");
            }

            if (ProvinceId <= 0)
            {
                return Result.Fail("Company.Address.Modify.Error", "未选择公司所在身份");
            }

            if (CityId <= 0) {
                return Result.Fail("Company.Address.Modify.Error", "未选择公司所在城市");
            }

            if (DistrictId <= 0) {
                return Result.Fail("Company.Address.Modify.Error", "未选择公司所在区县");
            }

            return Result.Ok();
        }
    }
}
