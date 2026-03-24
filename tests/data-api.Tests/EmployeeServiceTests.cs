using DataApi.Models;
using DataApi.Repositories;
using DataApi.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DataApi.Tests;

[TestClass]
public sealed class EmployeeServiceTests
{
    [TestMethod]
    public async Task GetEmployeesAsync_ReturnsPagedEnvelope()
    {
        var repo = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        repo.Setup(r => r.CountEmployeesAsync(null, null, It.IsAny<CancellationToken>())).ReturnsAsync(2);
        repo.Setup(r => r.GetEmployeesAsync(null, null, 0, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee>
            {
                new() { EmpId = 1, EmpName = "Alice" },
                new() { EmpId = 2, EmpName = "Bob" }
            });

        var svc = new EmployeeService(repo.Object);

        var result = await svc.GetEmployeesAsync(page: 1, pageSize: 100, empId: null, empname: null, CancellationToken.None);

        Assert.AreEqual(1, result.page);
        Assert.AreEqual(100, result.pageSize);
        Assert.AreEqual(2, result.totalCount);
        Assert.AreEqual(1, result.totalPages);
        Assert.AreEqual(2, result.items.Count);
        Assert.AreEqual(1, result.items[0].empId);
    }

    [TestMethod]
    public async Task GetEmployeesAsync_InvalidPage_Throws()
    {
        var repo = new Mock<IEmployeeRepository>(MockBehavior.Loose);
        var svc = new EmployeeService(repo.Object);

        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
            svc.GetEmployeesAsync(0, 10, null, null, CancellationToken.None));
    }
}
