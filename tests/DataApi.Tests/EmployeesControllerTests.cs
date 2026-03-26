using DataApi.Controllers;
using DataApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DataApi.Tests;

[TestClass]
public class EmployeesControllerTests
{
    [TestMethod]
    public async Task GetAll_ReturnsBadRequest_WhenPaginationInvalid()
    {
        var svc = new Mock<IEmployeeService>();
        var logger = new Mock<ILogger<EmployeesController>>();
        var controller = new EmployeesController(svc.Object, logger.Object);

        var result = await controller.GetAll(page: 0, pageSize: 10, CancellationToken.None);

        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task GetAll_ReturnsOk_WithPagedResult()
    {
        var svc = new Mock<IEmployeeService>();
        svc.Setup(s => s.GetEmployeesAsync(1, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<EmployeeDto>(
                new List<EmployeeDto> { new(1, "A") },
                1,
                100,
                1,
                1));

        var logger = new Mock<ILogger<EmployeesController>>();
        var controller = new EmployeesController(svc.Object, logger.Object);

        var result = await controller.GetAll(1, 100, CancellationToken.None);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var ok = (OkObjectResult)result.Result!;
        var payload = ok.Value as PagedResult<EmployeeDto>;
        Assert.IsNotNull(payload);
        Assert.AreEqual(1, payload!.Items.Count);
    }
}
