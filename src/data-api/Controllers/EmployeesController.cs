using DataApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private const int MaxPageSize = 200;

    private readonly IEmployeeService _service;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeService service, ILogger<EmployeesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0)
        {
            return BadRequest(new { error = new { code = "invalid_pagination", message = "page must be greater than 0." } });
        }

        if (pageSize <= 0 || pageSize > MaxPageSize)
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "invalid_pagination",
                    message = $"pageSize must be between 1 and {MaxPageSize}."
                }
            });
        }

        try
        {
            var result = await _service.GetEmployeesAsync(page, pageSize, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employees");
            throw;
        }
    }
}
