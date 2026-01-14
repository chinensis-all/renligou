using Renligou.Core.Application.Enterprise.Commands;
using Renligou.Core.Application.Kernel.Queries;
using Renligou.Core.Domain.CommonContext.Value;
using Renligou.Core.Domain.EnterpriseContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.Enterprise.Handlers
{
    public class ModifyCompanyAdressHandler(
        ICompanyRepository _companyStateRepository,
        IRegionQueryRepository _regionQueryRepository,
        IOutboxRepository _outboxRepository
    ) : ICommandHandler<ModifyCompanyAddressCommand, Result>
    {
        public async Task<Result> HandleAsync(ModifyCompanyAddressCommand command, CancellationToken cancellationToken)
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

            var regionNames = await _regionQueryRepository.QueryRegionNamesByIdsAsync(new List<long> { command.ProvinceId, command.CityId, command.DistrictId });
            var address = new Address(
                command.ProvinceId,
                regionNames.GetValueOrDefault(command.ProvinceId, string.Empty),
                command.CityId,
                regionNames.GetValueOrDefault(command.CityId, string.Empty),
                command.DistrictId,
                regionNames.GetValueOrDefault(command.DistrictId, string.Empty),
                command.CompletedAddress
            );
            company.ModifyAddress(address);

            await _companyStateRepository.SaveAsync(company);
            await _outboxRepository.AddAsync(company.GetRegisteredEvents(), "DOMAIN", company.GetType().Name, company.Id.id.ToString());

            return Result.Ok();
        }
    }
}
