using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.Enterprise.Commands
{
    public sealed record ModifyCompanyStateCommand : ICommand<Result> {

        public long CompanyId { get; init; }

        public bool Enabled { get; init; }

        public DateOnly? EffectiveDate { get; init; } =  null! ;

        public DateOnly? ExpiredDate { get; init; } = null!;

        public Result Validate()
        {
            if (CompanyId <= 0)
            {
                return Result.Fail("modify_company_state_error", "未选择公司");
            }

            return Result.Ok();
        }
    }
}
