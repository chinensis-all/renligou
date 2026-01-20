using Microsoft.EntityFrameworkCore;
using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.AuthorizationContext.Value;
using Renligou.Core.Infrastructure.Persistence.EFCore;
using Renligou.Core.Infrastructure.Persistence.Pos;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Infrastructure.Persistence.Repos;

public class DepartmentRepository(MysqlDbContext _db) : IDepartmentRepository, IDepartmentQueryRepository
{
    private readonly Dictionary<long, DepartmentPo> _tracked = new();

    public async Task<Department?> LoadAsync(long id)
    {
        var po = await _db.Departments.FindAsync(id);
        if (po == null) return null;

        _tracked[id] = po;
        return MapToAggregate(po);
    }

    public async Task SaveAsync(Department aggregate)
    {
        if (_tracked.TryGetValue(aggregate.Id.id, out var po))
        {
            ApplyAggregateToPo(aggregate, po);
            _db.Departments.Update(po);
        }
        else
        {
            po = new DepartmentPo();
            po.Id = aggregate.Id.id;
            ApplyAggregateToPo(aggregate, po);
            po.CreatedAt = DateTime.Now;
            po.UpdatedAt = DateTime.Now;
            await _db.Departments.AddAsync(po);
        }
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.Departments.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> IsCompanyDeptNameConflictAsync(long companyId, long parentId, string deptName, CancellationToken cancellationToken = default)
    {
        return await _db.Departments.AnyAsync(x => x.CompanyId == companyId && x.ParentId == parentId && x.DeptName == deptName, cancellationToken);
    }

    // IDepartmentQueryRepository Implementation
    public async Task<DepartmentDetailDto?> QueryDetailAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.Departments
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new DepartmentDetailDto
            {
                Id = x.Id.ToString(),
                ParentId = x.ParentId.ToString(),
                CompanyId = x.CompanyId.ToString(),
                DeptName = x.DeptName,
                DeptCode = x.DeptCode,
                Description = x.Description,
                Sorter = x.Sorter,
                Status = x.Status,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<DepartmentListDto>> SearchAsync(long? companyId, long? parentId, string? deptName, CancellationToken cancellationToken = default)
    {
        var query = _db.Departments.AsNoTracking();

        if (companyId.HasValue) query = query.Where(x => x.CompanyId == companyId.Value);
        if (parentId.HasValue) query = query.Where(x => x.ParentId == parentId.Value);
        if (!string.IsNullOrEmpty(deptName)) query = query.Where(x => x.DeptName.Contains(deptName));

        return await query
            .OrderBy(x => x.Sorter)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new DepartmentListDto
            {
                Id = x.Id.ToString(),
                ParentId = x.ParentId.ToString(),
                DeptName = x.DeptName,
                DeptCode = x.DeptCode,
                Sorter = x.Sorter,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<Pagination<DepartmentDetailDto>> PaginateAsync(int pageIndex, int pageSize, long? companyId, long? parentId, string? deptName, CancellationToken cancellationToken = default)
    {
        var query = _db.Departments.AsNoTracking();

        if (companyId.HasValue) query = query.Where(x => x.CompanyId == companyId.Value);
        if (parentId.HasValue) query = query.Where(x => x.ParentId == parentId.Value);
        if (!string.IsNullOrEmpty(deptName)) query = query.Where(x => x.DeptName.Contains(deptName));

        var total = await query.LongCountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Sorter)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DepartmentDetailDto
            {
                Id = x.Id.ToString(),
                ParentId = x.ParentId.ToString(),
                CompanyId = x.CompanyId.ToString(),
                DeptName = x.DeptName,
                DeptCode = x.DeptCode,
                Description = x.Description,
                Sorter = x.Sorter,
                Status = x.Status,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new Pagination<DepartmentDetailDto>
        {
            Page = pageIndex,
            PageSize = pageSize,
            Total = total,
            Items = items
        };
    }

    private Department MapToAggregate(DepartmentPo po)
    {
        return new Department(
            new AggregateId(po.Id, true),
            po.ParentId,
            po.CompanyId,
            po.DeptName,
            po.DeptCode,
            po.Description,
            po.Sorter,
            DepartmentStatus.FromCode(po.Status)
        );
    }

    private void ApplyAggregateToPo(Department aggregate, DepartmentPo po)
    {
        po.ParentId = aggregate.ParentId;
        po.CompanyId = aggregate.CompanyId;
        po.DeptName = aggregate.DeptName;
        po.DeptCode = aggregate.DeptCode;
        po.Description = aggregate.Description;
        po.Sorter = aggregate.Sorter;
        po.Status = aggregate.Status.Code;
        po.UpdatedAt = DateTime.Now;
    }
}
