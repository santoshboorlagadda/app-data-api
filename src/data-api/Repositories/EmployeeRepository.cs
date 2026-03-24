using DataApi.Data;
using DataApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DataApi.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly MysampleContext _db;

    public EmployeeRepository(MysampleContext db)
    {
        _db = db;
    }

    public async Task<List<Employee>> GetEmployeesAsync(int? empId, string? empNamePrefix, int skip, int take, CancellationToken cancellationToken)
    {
        return await BuildQuery(empId, empNamePrefix)
            .OrderBy(e => e.EmpId)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountEmployeesAsync(int? empId, string? empNamePrefix, CancellationToken cancellationToken)
    {
        return await BuildQuery(empId, empNamePrefix)
            .CountAsync(cancellationToken);
    }

    private IQueryable<Employee> BuildQuery(int? empId, string? empNamePrefix)
    {
        var query = _db.Employees.AsNoTracking();

        var hasEmpId = empId.HasValue;
        var hasName = !string.IsNullOrWhiteSpace(empNamePrefix);

        if (!hasEmpId && !hasName)
        {
            return query;
        }

        return query.Where(e =>
            (hasEmpId && e.EmpId == empId) ||
            (hasName && e.EmpName != null && EF.Functions.ILike(e.EmpName, $"{empNamePrefix}%")));
    }
}
