namespace DataApi.Services;

public interface IEmployeeService
{
    Task<PagedResponse<EmployeeDto>> GetEmployeesAsync(int page, int pageSize, int? empId, string? empname, CancellationToken cancellationToken);
}

public sealed record EmployeeDto(int empId, string? empName);

public sealed record PagedResponse<T>(
    IReadOnlyList<T> items,
    int page,
    int pageSize,
    int totalCount,
    int totalPages);
