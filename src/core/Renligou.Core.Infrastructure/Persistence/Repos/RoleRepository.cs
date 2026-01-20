using Microsoft.EntityFrameworkCore;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Infrastructure.Persistence.Pos;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Repo;

namespace Renligou.Core.Infrastructure.Persistence.Repos;

/// <summary>
/// 角色仓储实现 (领域仓储 + 查询仓储)
/// </summary>
public class RoleRepository(DbContext _db) : IRoleRepository, IRoleQueryRepository
{
    private readonly Dictionary<long, RolePo> _tracked = new();

    // --- IRoleRepository (Domain) ---

    public async Task<Role?> LoadAsync(long id)
    {
        var po = await _db.Set<RolePo>()
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == 0);

        if (po == null) return null;

        if (!_tracked.ContainsKey(id))
        {
            _tracked.Add(id, po);
        }

        return MapToAggregate(po);
    }

    public async Task SaveAsync(Role aggregate)
    {
        if (_tracked.TryGetValue(aggregate.Id.Id, out var po))
        {
            // 已存在，检查是否需要软删除
            var events = aggregate.GetRegisteredEvents();
            if (events.Any(e => e is Renligou.Core.AuthorizationContext.Event.RoleDestroyedEvent))
            {
                po.DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            else
            {
                ApplyAggregateToPo(aggregate, po);
            }
            po.UpdatedAt = DateTime.Now;
            _db.Update(po);
        }
        else
        {
            // 新增
            po = new RolePo
            {
                Id = aggregate.Id.Id,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                DeletedAt = 0
            };
            ApplyAggregateToPo(aggregate, po);
            await _db.AddAsync(po);
            _tracked.Add(po.Id, po);
        }
    }

    public async Task<bool> IsRoleNameConflictAsync(long id, string roleName)
    {
        return await _db.Set<RolePo>()
            .AnyAsync(x => x.Id != id && x.RoleName == roleName && x.DeletedAt == 0);
    }

    public async Task<bool> IsDisplayNameConflictAsync(long id, string displayName)
    {
        return await _db.Set<RolePo>()
            .AnyAsync(x => x.Id != id && x.DisplayName == displayName && x.DeletedAt == 0);
    }

    // --- IRoleQueryRepository (Query) ---

    public async Task<RoleDetailDto?> QueryDetailAsync(long roleId, CancellationToken cancellationToken = default)
    {
        return await _db.Set<RolePo>()
            .AsNoTracking()
            .Where(x => x.Id == roleId && x.DeletedAt == 0)
            .Select(x => new RoleDetailDto
            {
                Id = x.Id,
                RoleName = x.RoleName,
                DisplayName = x.DisplayName,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<RoleListDto>> SearchAsync(string? keyword, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<RolePo>().AsNoTracking().Where(x => x.DeletedAt == 0);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.RoleName.Contains(keyword) || x.DisplayName.Contains(keyword));
        }

        return await query.Select(x => new RoleListDto
        {
            Id = x.Id,
            RoleName = x.RoleName,
            DisplayName = x.DisplayName
        }).ToListAsync(cancellationToken);
    }

    public async Task<Pagination<RoleDetailDto>> PaginateAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<RolePo>().AsNoTracking().Where(x => x.DeletedAt == 0);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.RoleName.Contains(keyword) || x.DisplayName.Contains(keyword));
        }

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new RoleDetailDto
            {
                Id = x.Id,
                RoleName = x.RoleName,
                DisplayName = x.DisplayName,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new Pagination<RoleDetailDto>(items, total, page, pageSize);
    }

    // --- Helper Methods ---

    private static Role MapToAggregate(RolePo po)
    {
        // 使用反射或特定的私有构造函数/工厂模式来重建聚合根
        // 这里假设 Role 有一个接受所有必要参数的构造函数，或者通过私有设置器
        var aggregate = (Role)Activator.CreateInstance(typeof(Role), true)!;
        
        // 由于字段是 private set，需要使用反射或定义一个内部初始化方法
        // 为了遵循 Skill 里的“性能优先”和“减少反射”，通常建议在聚合根里提供一个 InternalLoad 方法
        // 或者直接在构造函数中处理。
        
        var idField = typeof(AggregateBase).GetProperty("Id")!;
        idField.SetValue(aggregate, new AggregateId(po.Id, false));

        var roleNameField = typeof(Role).GetProperty("RoleName")!;
        roleNameField.SetValue(aggregate, po.RoleName);

        var displayNameField = typeof(Role).GetProperty("DisplayName")!;
        displayNameField.SetValue(aggregate, po.DisplayName);

        return aggregate;
    }

    private static void ApplyAggregateToPo(Role aggregate, RolePo po)
    {
        po.RoleName = aggregate.RoleName;
        po.DisplayName = aggregate.DisplayName;
    }
}
