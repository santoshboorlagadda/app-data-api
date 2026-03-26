using DataApi.Repositories;

namespace DataApi.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repo;

    public EmployeeService(IEmployeeRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repo.GetEmployeesAsync(page, pageSize, cancellationToken);
        var dtoItems = items.Select(e => new EmployeeDto(e.EmpId, e.EmpName)).ToList();
        var totalPages = pageSize <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<EmployeeDto>(dtoItems, page, pageSize, totalCount, totalPages);
    }
}
