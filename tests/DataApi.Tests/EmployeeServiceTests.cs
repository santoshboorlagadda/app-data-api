using DataApi.Services;
using DataApi.Repositories;
using DataApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DataApi.Tests;

[TestClass]
public class EmployeeServiceTests
{
    [TestMethod]
    public async Task GetEmployeesAsync_MapsToDtos_AndComputesTotalPages()
    {
        var repo = new Mock<IEmployeeRepository>();
        repo.Setup(r => r.GetEmployeesAsync(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Employee>
            {
                new() { EmpId = 1, EmpName = "A" },
                new() { EmpId = 2, EmpName = "B" }
            }, 3));

        var service = new EmployeeService(repo.Object);

        var result = await service.GetEmployeesAsync(1, 2, CancellationToken.None);

        Assert.AreEqual(1, result.Page);
        Assert.AreEqual(2, result.PageSize);
        Assert.AreEqual(3, result.TotalCount);
        Assert.AreEqual(2, result.TotalPages);
        Assert.AreEqual(2, result.Items.Count);
        Assert.AreEqual(1, result.Items[0].EmpId);
    }
}
