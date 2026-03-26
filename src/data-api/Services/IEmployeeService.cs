namespace DataApi.Services;

public record EmployeeDto(int EmpId, string? EmpName);

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(int page, int pageSize, CancellationToken cancellationToken);
}
