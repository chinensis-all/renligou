using Microsoft.EntityFrameworkCore;
using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Infrastructure.Persistence.Pos;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Infrastructure.Persistence.Repos
{
    public class PermissionGroupRepository : IPermissionGroupRepository, IPermissionGroupQueryRepository
    {
        private readonly DbContext _db;
        private readonly Dictionary<long, PermissionGroupPo> _tracked = new();

        public PermissionGroupRepository(DbContext db)
        {
            _db = db;
        }

        public async Task<PermissionGroup?> LoadAsync(long id)
        {
            var po = await _db.Set<PermissionGroupPo>().FirstOrDefaultAsync(x => x.Id == id);
            if (po == null)
            {
                return null;
            }

            _tracked[id] = po;
            return MapToAggregate(po);
        }

        public async Task SaveAsync(PermissionGroup aggregate)
        {
            if (aggregate.Id.isNew)
            {
                var po = new PermissionGroupPo();
                ApplyAggregateToPo(aggregate, po);
                _db.Set<PermissionGroupPo>().Add(po);
                _db.Entry(po).State = EntityState.Added;
            }
            else
            {
                if (!_tracked.TryGetValue(aggregate.Id.id, out var po))
                {
                    // Fallback if not tracked (shouldn't happen if LoadAsync was called)
                    po = await _db.Set<PermissionGroupPo>().FindAsync(aggregate.Id.id);
                    if (po == null) throw new InvalidOperationException($"PermissionGroup with id {aggregate.Id.id} not found.");
                }
                
                ApplyAggregateToPo(aggregate, po);
                _db.Set<PermissionGroupPo>().Update(po);
            }
        }

        public async Task<bool> IsGroupNameConflictAsync(long id, string groupName)
        {
            return await _db.Set<PermissionGroupPo>()
                .AnyAsync(x => x.GroupName == groupName && x.Id != id && x.DeletedAt == 0);
        }

        public async Task<bool> IsDisplayNameConflictAsync(long id, string displayName)
        {
            return await _db.Set<PermissionGroupPo>()
                .AnyAsync(x => x.DisplayName == displayName && x.Id != id && x.DeletedAt == 0);
        }

        public Task<PermissionGroupDetailDto?> QueryDetailAsync(long id, CancellationToken cancellationToken = default)
        {
            return _db.Set<PermissionGroupPo>()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new PermissionGroupDetailDto
                {
                    Id = x.Id.ToString(),
                    GroupName = x.GroupName,
                    DisplayName = x.DisplayName,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = x.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<PermissionGroupListDto>> SearchAsync(PermissionGroupSearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            return GetSearchQuery(criteria)
                .Select(x => new PermissionGroupListDto
                {
                    Id = x.Id.ToString(),
                    GroupName = x.GroupName,
                    DisplayName = x.DisplayName
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<Pagination<PermissionGroupDetailDto>> PaginateAsync(PermissionGroupSearchCriteria searchCriteria, PermissionGroupPaginateCriteria paginateCriteria, CancellationToken cancellationToken = default)
        {
            var query = GetSearchQuery(searchCriteria);
            var total = await query.LongCountAsync(cancellationToken);

            if (total == 0)
            {
                return new Pagination<PermissionGroupDetailDto>
                {
                    Page = paginateCriteria.Page,
                    PageSize = paginateCriteria.PageSize,
                    Total = 0,
                    Items = new List<PermissionGroupDetailDto>()
                };
            }

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((paginateCriteria.Page - 1) * paginateCriteria.PageSize)
                .Take(paginateCriteria.PageSize)
                .Select(x => new PermissionGroupDetailDto
                {
                    Id = x.Id.ToString(),
                    GroupName = x.GroupName,
                    DisplayName = x.DisplayName,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = x.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync(cancellationToken);

            return new Pagination<PermissionGroupDetailDto>
            {
                Page = paginateCriteria.Page,
                PageSize = paginateCriteria.PageSize,
                Total = total,
                Items = items
            };
        }

        private IQueryable<PermissionGroupPo> GetSearchQuery(PermissionGroupSearchCriteria criteria)
        {
            var query = _db.Set<PermissionGroupPo>().AsNoTracking().Where(x => x.DeletedAt == 0);

            if (criteria != null)
            {
                if (!string.IsNullOrWhiteSpace(criteria.GroupName))
                {
                    query = query.Where(x => x.GroupName.Contains(criteria.GroupName));
                }

                if (!string.IsNullOrWhiteSpace(criteria.DisplayName))
                {
                    query = query.Where(x => x.DisplayName.Contains(criteria.DisplayName));
                }
            }

            return query;
        }

        private PermissionGroup MapToAggregate(PermissionGroupPo po)
        {
            return new PermissionGroup(
                new AggregateId(po.Id, false),
                po.GroupName,
                po.DisplayName,
                po.Description,
                po.DeletedAt
            );
        }

        private void ApplyAggregateToPo(PermissionGroup aggregate, PermissionGroupPo po)
        {
            po.Id = aggregate.Id.id;
            po.GroupName = aggregate.GroupName;
            po.DisplayName = aggregate.DisplayName;
            po.Description = aggregate.Description;
            po.DeletedAt = aggregate.DeletedAt;

            if (aggregate.Id.isNew)
            {
                po.CreatedAt = DateTime.UtcNow;
            }
            po.UpdatedAt = DateTime.UtcNow;
        }
    }
}
