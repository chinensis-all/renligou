using Microsoft.EntityFrameworkCore;
using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Infrastructure.Persistence.Pos;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Infrastructure.Persistence.Repos
{
    public class PermissionRepository : IPermissionRepository, IPermissionQueryRepository
    {
        private readonly DbContext _db;
        private readonly Dictionary<long, PermissionPo> _tracked = new();

        public PermissionRepository(DbContext db)
        {
            _db = db;
        }

        public async Task<Permission?> LoadAsync(long id)
        {
            var po = await _db.Set<PermissionPo>().FirstOrDefaultAsync(x => x.Id == id);
            if (po == null)
            {
                return null;
            }

            _tracked[id] = po;
            return MapToAggregate(po);
        }

        public async Task SaveAsync(Permission aggregate)
        {
            if (aggregate.Id.isNew)
            {
                var po = new PermissionPo();
                ApplyAggregateToPo(aggregate, po);
                _db.Set<PermissionPo>().Add(po);
            }
            else
            {
                if (!_tracked.TryGetValue(aggregate.Id.id, out var po))
                {
                    po = await _db.Set<PermissionPo>().FindAsync(aggregate.Id.id);
                    if (po == null) throw new InvalidOperationException($"Permission with id {aggregate.Id.id} not found.");
                }

                ApplyAggregateToPo(aggregate, po);
                _db.Set<PermissionPo>().Update(po);
            }
        }

        public async Task<bool> IsPermissionNameConflictAsync(long id, string permissionName)
        {
            return await _db.Set<PermissionPo>()
                .AnyAsync(x => x.PermissionName == permissionName && x.Id != id && x.DeletedAt == 0);
        }

        public async Task<bool> IsDisplayNameConflictAsync(long id, string displayName)
        {
            return await _db.Set<PermissionPo>()
                .AnyAsync(x => x.DisplayName == displayName && x.Id != id && x.DeletedAt == 0);
        }

        public async Task<bool> HasPermissionsAsync(long groupId, CancellationToken cancellationToken)
        {
            // 获取该组及其所有子代的ID
            var allGroupIds = await GetGroupAndDescendantIdsAsync(groupId, cancellationToken);
            
            // 检查这些组下是否存在权限
            return await _db.Set<PermissionPo>()
                .AnyAsync(x => allGroupIds.Contains(x.GroupId) && x.DeletedAt == 0, cancellationToken);
        }

        private async Task<List<long>> GetGroupAndDescendantIdsAsync(long rootGroupId, CancellationToken cancellationToken)
        {
            // 加载所有未删除的权限组以在内存中构建搜索（简单实现，适用于权限组数量不大的情况）
            var allGroups = await _db.Set<PermissionGroupPo>()
                .AsNoTracking()
                .Where(x => x.DeletedAt == 0)
                .Select(x => new { x.Id, x.ParentId })
                .ToListAsync(cancellationToken);

            var result = new List<long> { rootGroupId };
            var queue = new Queue<long>();
            queue.Enqueue(rootGroupId);

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                var children = allGroups.Where(x => x.ParentId == currentId).Select(x => x.Id);
                foreach (var childId in children)
                {
                    if (!result.Contains(childId))
                    {
                        result.Add(childId);
                        queue.Enqueue(childId);
                    }
                }
            }

            return result;
        }

        public async Task<PermissionDetailDto?> QueryDetailAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _db.Set<PermissionPo>()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new PermissionDetailDto
                {
                    Id = x.Id.ToString(),
                    GroupId = x.GroupId.ToString(),
                    PermissionName = x.PermissionName,
                    DisplayName = x.DisplayName,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = x.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountAsync(PermissionSearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            return await GetSearchQuery(criteria).LongCountAsync(cancellationToken);
        }

        public async Task<List<PermissionListDto>> SearchAsync(PermissionSearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            return await GetSearchQuery(criteria)
                .Select(x => new PermissionListDto
                {
                    Id = x.Id.ToString(),
                    GroupId = x.GroupId.ToString(),
                    PermissionName = x.PermissionName,
                    DisplayName = x.DisplayName
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<Pagination<PermissionDetailDto>> PaginateAsync(PermissionSearchCriteria searchCriteria, PermissionPaginateCriteria paginateCriteria, CancellationToken cancellationToken = default)
        {
            var query = GetSearchQuery(searchCriteria);
            var total = await query.LongCountAsync(cancellationToken);

            if (total == 0)
            {
                return new Pagination<PermissionDetailDto>
                {
                    Page = paginateCriteria.Page,
                    PageSize = paginateCriteria.PageSize,
                    Total = 0,
                    Items = new List<PermissionDetailDto>()
                };
            }

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((paginateCriteria.Page - 1) * paginateCriteria.PageSize)
                .Take(paginateCriteria.PageSize)
                .Select(x => new PermissionDetailDto
                {
                    Id = x.Id.ToString(),
                    GroupId = x.GroupId.ToString(),
                    PermissionName = x.PermissionName,
                    DisplayName = x.DisplayName,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = x.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync(cancellationToken);

            return new Pagination<PermissionDetailDto>
            {
                Page = paginateCriteria.Page,
                PageSize = paginateCriteria.PageSize,
                Total = total,
                Items = items
            };
        }

        private IQueryable<PermissionPo> GetSearchQuery(PermissionSearchCriteria criteria)
        {
            var query = _db.Set<PermissionPo>().AsNoTracking().Where(x => x.DeletedAt == 0);

            if (criteria != null)
            {
                if (criteria.GroupId.HasValue)
                {
                    query = query.Where(x => x.GroupId == criteria.GroupId.Value);
                }

                if (!string.IsNullOrWhiteSpace(criteria.PermissionName))
                {
                    query = query.Where(x => x.PermissionName.Contains(criteria.PermissionName));
                }

                if (!string.IsNullOrWhiteSpace(criteria.DisplayName))
                {
                    query = query.Where(x => x.DisplayName.Contains(criteria.DisplayName));
                }
            }

            return query;
        }

        private Permission MapToAggregate(PermissionPo po)
        {
            return new Permission(
                new AggregateId(po.Id, false),
                po.GroupId,
                po.PermissionName,
                po.DisplayName,
                po.Description,
                po.DeletedAt
            );
        }

        private void ApplyAggregateToPo(Permission aggregate, PermissionPo po)
        {
            po.Id = aggregate.Id.id;
            po.GroupId = aggregate.GroupId;
            po.PermissionName = aggregate.PermissionName;
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
