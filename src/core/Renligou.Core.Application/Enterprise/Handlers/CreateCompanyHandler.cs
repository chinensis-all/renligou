using Renligou.Core.Application.Enterprise.Commands;
using Renligou.Core.Application.Kernel.Queries;
using Renligou.Core.Domain.CommonContext.Value;
using Renligou.Core.Domain.EnterpriseContext.Model;
using Renligou.Core.Domain.EnterpriseContext.Repo;
using Renligou.Core.Domain.EnterpriseContext.Value;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.Enterprise.Handlers
{
    public class CreateCompanyHandler(
        ICompanyRepository _companyRepository,
        IRegionQueryRepository _regionQueryRepository,
        IOutboxRepository _outboxRepository,
        IIdGenerator _idGenerator
    ) : ICommandHandler<CreateCompanyCommand, Result>
    {
        public async Task<Result> HandleAsync(CreateCompanyCommand command, CancellationToken cancellationToken)
        {
            var validation = command.Validate();
            if (!validation.Success)
            {
                return validation;
            }

            long id = _idGenerator.NextId();

            var companyNameExits = await _companyRepository.CompanyNameExistsAsync(id, command.CompanyName);
            if (companyNameExits)
            {
                return Result.Fail("Company.Create.Error", "公司名称已存在");
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

            var state = new CompanyState(
                command.Enabled,
                command.EffectiveDate,
                command.ExpiredDate
            );

            var company = new Company(
                new AggregateId(id, true),
                Enum.Parse<CompanyType>(command.CompanyType, ignoreCase: true),
                command.CompanyCode,
                command.CompanyName,
                command.CompanyShortName,
                command.LegalPersonName,
                command.CreditCode,
                command.RegisteredAddress,
                command.Remark,
                address,
                state
            );
            company.Create();

            await _companyRepository.SaveAsync(company);
            await _outboxRepository.AddAsync(company.GetRegisteredEvents(), "DOMAIN", company.GetType().Name, company.Id.id.ToString());

            return Result.Ok();
        }
    }
}
