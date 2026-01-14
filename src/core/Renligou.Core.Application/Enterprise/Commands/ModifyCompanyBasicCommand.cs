using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.Enterprise.Commands
{
    public sealed record ModifyCompanyBasicCommand : ICommand<Result> {

        public long CompanyId { get; init; }

        public string CompanyType { get; init; }

        public string CompanyCode { get; init; }

        public string CompanyName { get; init; }

        public string CompanyShortName { get; init; }

        public string LegalPersonName { get; init; }

        public string CreditCode { get; init; }

        public string RegisteredAddress { get; init; }

        public string Remark { get; init; }

        public Result Validate()
        {
            if (CompanyId <= 0)
            {
                return Result.Fail("Company.Basic.Modify.Error", "未选择公司");
            }

            if (string.IsNullOrEmpty(CompanyName))
            {
                return Result.Fail("Company.Basic.Modify.Error", "公司名称不能为空");
            }

            return Result.Ok();
        }
    }
}
