using Renligou.Core.Application.Enterprise.Commands;
using Renligou.Core.Domain.EnterpriseContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.Enterprise.Handlers
{
    public class ModifyCompanyBasicHandler(
        ICompanyRepository _companyStateRepository,
        IOutboxRepository _outboxRepository
    ) : ICommandHandler<ModifyCompanyBasicCommand, Result> {
        public async Task<Result> HandleAsync(ModifyCompanyBasicCommand command, CancellationToken cancellationToken)
        {
            var validation = command.Validate();
            if (!validation.Success)
            {
                return validation;
            }

            var company = await _companyStateRepository.LoadAsync(command.CompanyId);
            if (company == null)
            {
                return Result.Fail("modify_company_state_error", $"没有找到公司: {command.CompanyId}");
            }

            company.ModifyBasic(
                company.CompanyType,
                command.CompanyCode,
                command.CompanyName,
                command.CompanyShortName,
                command.LegalPersonName,
                command.CreditCode,
                command.RegisteredAddress,
                command.Remark
            );

            await _companyStateRepository.SaveAsync(company);
            await _outboxRepository.AddAsync(company.GetRegisteredEvents(), "DOMAIN", company.GetType().Name, company.Id.id.ToString());

            return Result.Ok();
        }
    }
}
