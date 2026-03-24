using DataApi.Controllers;
using DataApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DataApi.Tests;

[TestClass]
public sealed class EmployeesControllerTests
{
    [TestMethod]
    public async Task GetAll_InvalidPaging_ReturnsBadRequest()
    {
        var service = new Mock<IEmployeeService>(MockBehavior.Strict);
        var logger = Mock.Of<ILogger<EmployeesController>>();

        var controller = new EmployeesController(service.Object, logger);

        var result = await controller.GetAll(page: 0, pageSize: 10, empId: null, empname: null, CancellationToken.None);

        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task GetAll_ValidPaging_ReturnsOk()
    {
        var service = new Mock<IEmployeeService>(MockBehavior.Strict);
        service.Setup(s => s.GetEmployeesAsync(1, 100, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResponse<EmployeeDto>(new List<EmployeeDto>(), 1, 100, 0, 0));

        var logger = Mock.Of<ILogger<EmployeesController>>();
        var controller = new EmployeesController(service.Object, logger);

        var result = await controller.GetAll(page: 1, pageSize: 100, empId: null, empname: null, CancellationToken.None);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }
}
