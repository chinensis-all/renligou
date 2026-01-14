using Microsoft.EntityFrameworkCore;
using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.Enterprise.Criterias;
using Renligou.Core.Application.Enterprise.Queries;
using Renligou.Core.Domain.CommonContext.Value;
using Renligou.Core.Domain.EnterpriseContext.Model;
using Renligou.Core.Domain.EnterpriseContext.Repo;
using Renligou.Core.Domain.EnterpriseContext.Value;
using Renligou.Core.Infrastructure.Persistence.Pos;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Infrastructure.Persistence.Repos
{
    public class CompanyRepository : ICompanyRepository, ICompanyQueryRepository
    {
        private readonly DbContext _db;

        private readonly Dictionary<long, CompanyPo> _tracked = new();

        public CompanyRepository(DbContext db)
        {
            _db = db;
        }

        public async Task<Company?> LoadAsync(long id)
        {
            CompanyPo? po = await GetPoAsync(id);
            if (po == null)
            {
                return null;
            }

            _tracked[id] = po;

            return MapToAggregate(po);
        }

        public async Task SaveAsync(Company aggregate)
        {
            if (aggregate.Id.isNew)
            {
                var po = new CompanyPo();
                ApplyAggregateToPo(aggregate, po);
                _db.Set<CompanyPo>().Add(po);
                _db.Entry(po).State = EntityState.Added;
                return;
            }

            if (!_tracked.TryGetValue(aggregate.Id.id, out var entity))
                throw new InvalidOperationException("Aggregate not loaded");

            ApplyAggregateToPo(aggregate, entity);
            _db.Set<CompanyPo>().Update(entity);
        }

        private async Task<CompanyPo?> GetPoAsync(long id)
        {
            if (id <= 0)
            {
                return null;
            }

            return await _db.Set<CompanyPo>().FirstOrDefaultAsync(e => e.Id == id);
        }

        private Company MapToAggregate(CompanyPo po)
        {
            Address address = new Address
            {
                ProvinceId = po.ProvinceId,
                Province = po.Province,
                CityId = po.CityId,
                City = po.City,
                DistrictId = po.DistrictId,
                District = po.District,
                CompletedAddress = po.CompletedAddress
            };

            CompanyState state = new CompanyState
            {
                Enabled = po.Enabled == 1,
                EffectiveDate = po.EffectiveDate,
                ExpiredDate = po.ExpiredDate
            };

            return new Company(
                new AggregateId(po.Id, false),
                Enum.Parse<CompanyType>(po.CompanyType, ignoreCase: true),
                po.CompanyCode,
                po.CompanyName,
                po.CompanyShortName,
                po.LegalPersonName,
                po.CreditCode,
                po.RegisteredAddress,
                po.Remark,
                address,
                state
            );
        }

        private void ApplyAggregateToPo(Company aggregate, CompanyPo po)
        {
            po.Id = aggregate.Id.id;
            po.CompanyType = aggregate.CompanyType.ToString().ToUpper();
            po.CompanyCode = aggregate.CompanyCode;
            po.CompanyName = aggregate.CompanyName;
            po.CompanyShortName = aggregate.CompanyShortName ?? "";
            po.LegalPersonName = aggregate.LegalPersonName;
            po.CreditCode = aggregate.CreditCode;
            po.RegisteredAddress = aggregate.RegisteredAddress;

            po.ProvinceId = aggregate.Address.ProvinceId;
            po.Province = aggregate.Address.Province;
            po.CityId = aggregate.Address.CityId;
            po.City = aggregate.Address.City;
            po.DistrictId = aggregate.Address.DistrictId;
            po.District = aggregate.Address.District;
            po.CompletedAddress = aggregate.Address.CompletedAddress;

            po.Enabled = aggregate.State.Enabled ? (short)1 : (short)0;
            po.EffectiveDate = aggregate.State.EffectiveDate;
            po.ExpiredDate = aggregate.State.ExpiredDate;

            po.Remark = aggregate.Remark;

            po.CreatedAt = aggregate.Id.isNew ? DateTimeOffset.UtcNow : po.CreatedAt;
            po.UpdatedAt = DateTimeOffset.UtcNow;
        }

        public async Task<bool> CompanyNameExistsAsync(long companyId, string companyName)
        {
            return await _db.Set<CompanyPo>().AnyAsync(e => e.CompanyName == companyName && e.Id != companyId);
        }

        public Task<CompanyDetailDto?> QueryDetailAsync(long companyId, CancellationToken cancellationToken = default)
        {
            return _db.Set<CompanyPo>()
                .Where(e => e.Id == companyId)
                .Select(e => new CompanyDetailDto
                {
                    CompanyId = e.Id.ToString(),
                    CompanyType = e.CompanyType,
                    CompanyName = e.CompanyName,
                    CompanyShortName = e.CompanyShortName,
                    LegalPersonName = e.LegalPersonName,
                    CreditCode = e.CreditCode,
                    RegisteredAddress = e.RegisteredAddress,
                    CreatedAt = e.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = e.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    ProvinceId = e.ProvinceId.ToString(),
                    Province = e.Province,
                    CityId = e.CityId.ToString(),
                    City = e.City,
                    DistrictId = e.DistrictId.ToString(),
                    District = e.District,
                    CompletedAddress = e.CompletedAddress,
                    Enabled = (e.Enabled == 1),
                    EffectiveDate = e.EffectiveDate.HasValue ? e.EffectiveDate.Value.ToString("yyyy-MM-dd") : "",
                    ExpiredDate = e.ExpiredDate.HasValue ? e.ExpiredDate.Value.ToString("yyyy-MM-dd") : "",
                    Remark = e.Remark
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountAsync(CompanySearchCriteria companyCriteria, CancellationToken cancellationToken = default)
        {
            return await GetSearchQuery(companyCriteria, cancellationToken).CountAsync(cancellationToken);
        }

        public Task<List<CompanyListDto>> SearchAsync(CompanySearchCriteria companyCriteria, CancellationToken cancellationToken = default)
        {
            return GetSearchQuery(companyCriteria, cancellationToken)
                .Select(e => new CompanyListDto
                {
                    CompanyId = e.Id.ToString(),
                    CompanyCode = e.CompanyCode,
                    CompanyName = e.CompanyName,
                    CompanyShortName = e.CompanyShortName,
                    CompanyType = e.CompanyType
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<Pagination<CompanyDetailDto>> PaginateAsync(CompanySearchCriteria companyCriteria, CompanyPaginateCriteria paginateCriteria, CancellationToken cancellationToken = default)
        {
            long total = GetSearchQuery(companyCriteria, cancellationToken).LongCount();
            if (total == 0)
            {
                return new Pagination<CompanyDetailDto>
                {
                    Page = paginateCriteria.Page,
                    PageSize = paginateCriteria.PageSize,
                    Total = 0,
                    Items = new List<CompanyDetailDto>()
                };
            }

            var items = GetSearchQuery(companyCriteria, cancellationToken)
                .Skip((paginateCriteria.Page - 1) * paginateCriteria.PageSize)
                .Take(paginateCriteria.PageSize)
                .Select(e => new CompanyDetailDto
                {
                    CompanyId = e.Id.ToString(),
                    CompanyType = e.CompanyType,
                    CompanyName = e.CompanyName,
                    CompanyShortName = e.CompanyShortName,
                    LegalPersonName = e.LegalPersonName,
                    CreditCode = e.CreditCode,
                    RegisteredAddress = e.RegisteredAddress,
                    CreatedAt = e.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = e.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    ProvinceId = e.ProvinceId.ToString(),
                    Province = e.Province,
                    CityId = e.CityId.ToString(),
                    City = e.City,
                    DistrictId = e.DistrictId.ToString(),
                    District = e.District,
                    CompletedAddress = e.CompletedAddress,
                    Enabled = (e.Enabled == 1),
                    EffectiveDate = e.EffectiveDate.HasValue ? e.EffectiveDate.Value.ToString("yyyy-MM-dd") : "",
                    ExpiredDate = e.ExpiredDate.HasValue ? e.ExpiredDate.Value.ToString("yyyy-MM-dd") : "",
                    Remark = e.Remark
                });

            return new Pagination<CompanyDetailDto>
            {
                Page = paginateCriteria.Page,
                PageSize = paginateCriteria.PageSize,
                Total = total,
                Items = await items.ToListAsync(cancellationToken)
            };
        }

        private IQueryable<CompanyPo> GetSearchQuery(CompanySearchCriteria companyCriteria, CancellationToken cancellationToken = default)
        {
            var query = _db.Set<CompanyPo>().AsQueryable();

            if (companyCriteria == null)
            {
                return query;
            }

            if (!string.IsNullOrWhiteSpace(companyCriteria.CompanyType))
            {
                query = query.Where(e => e.CompanyType == companyCriteria.CompanyType);
            }

            if (!string.IsNullOrWhiteSpace(companyCriteria.CompanyName))
            {
                query = query.Where(e => e.CompanyName.Contains(companyCriteria.CompanyName));
            }

            if (companyCriteria.ProvinceId.HasValue)
            {
                query = query.Where(e => e.ProvinceId == companyCriteria.ProvinceId.Value);
            }

            if (!string.IsNullOrWhiteSpace(companyCriteria.Status))
            {
                if (companyCriteria.Status == "ENABLED")
                {
                    query = query.Where(e => e.Enabled == 1);
                }
                else if (companyCriteria.Status == "DISABLED")
                {
                    query = query.Where(e => e.Enabled == 0);
                }
            }

            return query;
        }
    }
}
