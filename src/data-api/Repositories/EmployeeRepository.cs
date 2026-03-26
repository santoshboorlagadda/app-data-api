using DataApi.Data;
using DataApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DataApi.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetEmployeesAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _db.Employees
            .AsNoTracking()
            .OrderBy(e => e.EmpId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
