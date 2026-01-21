using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.AuthorizationContext.Value;
using Renligou.Core.Infrastructure.Persistence.Pos;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Infrastructure.Persistence.Repos;

/// <summary>
/// 部门仓储实现 (领域仓储 + 查询仓储)
/// </summary>
public class DepartmentRepository(DbContext _db) : IDepartmentRepository, IDepartmentQueryRepository
{
    private readonly Dictionary<long, DepartmentPo> _tracked = new();

    // --- IDepartmentRepository (Domain) ---

    public async Task<Department?> LoadAsync(long id)
    {
        var po = await _db.Set<DepartmentPo>()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (po == null) return null;

        if (!_tracked.ContainsKey(id))
        {
            _tracked.Add(id, po);
        }

        return MapToAggregate(po);
    }

    public async Task SaveAsync(Department aggregate)
    {
        if (_tracked.TryGetValue(aggregate.Id.id, out var po))
        {
            ApplyAggregateToPo(aggregate, po);
            po.UpdatedAt = DateTimeOffset.UtcNow;
            _db.Update(po);
        }
        else
        {
            po = new DepartmentPo
            {
                Id = aggregate.Id.id,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            ApplyAggregateToPo(aggregate, po);
            await _db.AddAsync(po);
            _tracked.Add(po.Id, po);
        }
    }

    public async Task<bool> IsDeptNameConflictAsync(long companyId, long parentId, string deptName, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<DepartmentPo>().AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.ParentId == parentId && x.DeptName == deptName);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.Set<DepartmentPo>().AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken);
    }

    // --- IDepartmentQueryRepository (Query) ---

    public async Task<DepartmentDetailDto?> QueryDetailAsync(long departmentId, CancellationToken cancellationToken = default)
    {
        return await _db.Set<DepartmentPo>().AsNoTracking()
            .Where(x => x.Id == departmentId)
            .Select(x => new DepartmentDetailDto
            {
                Id = x.Id,
                ParentId = x.ParentId,
                CompanyId = x.CompanyId,
                DeptName = x.DeptName,
                DeptCode = x.DeptCode,
                Description = x.Description,
                Sorter = x.Sorter,
                Status = x.Status,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<DepartmentListDto>> SearchAsync(long? companyId, string? deptName, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<DepartmentPo>().AsNoTracking();

        if (companyId.HasValue)
        {
            query = query.Where(x => x.CompanyId == companyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(deptName))
        {
            query = query.Where(x => x.DeptName.Contains(deptName));
        }

        return await query.OrderBy(x => x.Sorter).Select(x => new DepartmentListDto
        {
            Id = x.Id,
            DeptName = x.DeptName,
            DeptCode = x.DeptCode,
            Status = x.Status,
            Sorter = x.Sorter
        }).ToListAsync(cancellationToken);
    }

    public async Task<List<DepartmentTreeNodeDto>> GetDepartmentTreeAsync(DepartmentTreeCriteria criteria, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<DepartmentPo>().AsNoTracking();

        if (criteria.CompanyId.HasValue)
        {
            query = query.Where(x => x.CompanyId == criteria.CompanyId.Value);
        }

        // 注意：树形结构通常需要加载所有节点来构建完整层级
        // 如果有名称搜索，通常是前端过滤或者后端返回匹配节点的所有祖先和后代，
        // 这里采用简单方案：如果没有名称搜索，从指定ParentId开始构建树；如果有名称搜索，返回匹配节点及其层级。
        // 但根据技能文档要求：查询所有结果后，构建树形结构。
        
        var allPos = await query.ToListAsync(cancellationToken);

        var allNodes = allPos.Select(x => new DepartmentTreeNodeDto
        {
            Id = x.Id,
            ParentId = x.ParentId,
            Name = x.DeptName
        }).ToList();

        if (!string.IsNullOrWhiteSpace(criteria.Name))
        {
            allNodes = allNodes.Where(x => x.Name.Contains(criteria.Name)).ToList();
            // 如果是搜索，通常返回扁平化结果或者特定结构的树，这里按技能说明构建。
        }

        return BuildTree(allNodes, criteria.ParentId);
    }

    private List<DepartmentTreeNodeDto> BuildTree(List<DepartmentTreeNodeDto> nodes, long parentId)
    {
        var result = new List<DepartmentTreeNodeDto>();
        var children = nodes.Where(x => x.ParentId == parentId).OrderBy(x => x.Id).ToList();
        foreach (var child in children)
        {
            var treeNode = new DepartmentTreeNodeDto
            {
                Id = child.Id,
                ParentId = child.ParentId,
                Name = child.Name
            };
            treeNode.Children.AddRange(BuildTree(nodes, child.Id));
            result.Add(treeNode);
        }
        return result;
    }

    // --- Helper Methods ---

    private Department MapToAggregate(DepartmentPo po)
    {
        // 使用反射或私有构造函数创建聚合根
        var department = (Department)Activator.CreateInstance(typeof(Department), true)!;
        
        // 设置属性
        typeof(Department).GetProperty(nameof(Department.Id))!.SetValue(department, new AggregateId(po.Id, true));
        typeof(Department).GetProperty(nameof(Department.ParentId))!.SetValue(department, po.ParentId);
        typeof(Department).GetProperty(nameof(Department.CompanyId))!.SetValue(department, po.CompanyId);
        typeof(Department).GetProperty(nameof(Department.DeptName))!.SetValue(department, po.DeptName);
        typeof(Department).GetProperty(nameof(Department.DeptCode))!.SetValue(department, po.DeptCode);
        typeof(Department).GetProperty(nameof(Department.Description))!.SetValue(department, po.Description);
        typeof(Department).GetProperty(nameof(Department.Sorter))!.SetValue(department, po.Sorter);
        typeof(Department).GetProperty(nameof(Department.Status))!.SetValue(department, DepartmentStatus.FromCode(po.Status));

        return department;
    }

    private static void ApplyAggregateToPo(Department aggregate, DepartmentPo po)
    {
        po.ParentId = aggregate.ParentId;
        po.CompanyId = aggregate.CompanyId;
        po.DeptName = aggregate.DeptName;
        po.DeptCode = aggregate.DeptCode;
        po.Description = aggregate.Description;
        po.Sorter = aggregate.Sorter;
        po.Status = aggregate.Status.Code;
    }
}
