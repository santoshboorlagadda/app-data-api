using DataApi.Models;

namespace DataApi.Repositories;

public interface IEmployeeRepository
{
    Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetEmployeesAsync(int page, int pageSize, CancellationToken cancellationToken);
}
