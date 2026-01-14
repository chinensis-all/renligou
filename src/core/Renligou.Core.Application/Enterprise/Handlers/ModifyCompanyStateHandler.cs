using Renligou.Core.Application.Enterprise.Commands;
using Renligou.Core.Domain.EnterpriseContext.Repo;
using Renligou.Core.Domain.EnterpriseContext.Value;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.Enterprise.Handlers
{
    public class ModifyCompanyStateHandler(
        ICompanyRepository _companyStateRepository,
        IOutboxRepository _outboxRepository
    ) : ICommandHandler<ModifyCompanyStateCommand, Result> {
        public async Task<Result> HandleAsync(ModifyCompanyStateCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();
            if (!validationResult.Success)
            {
                return validationResult;
            }

            var company = await _companyStateRepository.LoadAsync(command.CompanyId);
            if (company == null)
            {
                return Result.Fail("modify_company_state_error", $"没有找到公司: {command.CompanyId}");
            }

            var newState = new CompanyState(command.Enabled, command.EffectiveDate, command.ExpiredDate);
            company.ModifyState(newState);

            await _companyStateRepository.SaveAsync(company);
            await _outboxRepository.AddAsync(company.GetRegisteredEvents(), "DOMAIN", company.GetType().Name, company.Id.id.ToString());

            return Result.Ok();
        }
    }
}
