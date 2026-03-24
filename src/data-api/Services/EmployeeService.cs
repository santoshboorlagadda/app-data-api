using DataApi.Repositories;

namespace DataApi.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repo;

    public EmployeeService(IEmployeeRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResponse<EmployeeDto>> GetEmployeesAsync(int page, int pageSize, int? empId, string? empname, CancellationToken cancellationToken)
    {
        if (page <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "page must be > 0");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "pageSize must be > 0");
        }

        var skip = (page - 1) * pageSize;

        var totalCount = await _repo.CountEmployeesAsync(empId, empname, cancellationToken);
        var employees = await _repo.GetEmployeesAsync(empId, empname, skip, pageSize, cancellationToken);

        var items = employees.Select(e => new EmployeeDto(e.EmpId, e.EmpName)).ToList();
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResponse<EmployeeDto>(items, page, pageSize, totalCount, totalPages);
    }
}
