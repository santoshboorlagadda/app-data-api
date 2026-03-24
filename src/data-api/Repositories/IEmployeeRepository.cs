using DataApi.Models;

namespace DataApi.Repositories;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetEmployeesAsync(int? empId, string? empNamePrefix, int skip, int take, CancellationToken cancellationToken);
    Task<int> CountEmployeesAsync(int? empId, string? empNamePrefix, CancellationToken cancellationToken);
}
