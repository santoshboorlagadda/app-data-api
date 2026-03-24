using DataApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeService service, ILogger<EmployeesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<EmployeeDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        [FromQuery] int? empId = null,
        [FromQuery(Name = "empname")] string? empname = null,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest(new { error = "invalid_paging", message = "page and pageSize must be > 0" });
        }

        _logger.LogInformation("Fetching employees page={Page} pageSize={PageSize} empId={EmpId} empname={EmpName}", page, pageSize, empId, empname);
        var result = await _service.GetEmployeesAsync(page, pageSize, empId, empname, cancellationToken);
        return Ok(result);
    }
}
