using DataApi.Data;
using DataApi.Models;
using DataApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataApi.Tests;

[TestClass]
public class EmployeeRepositoryTests
{
    [TestMethod]
    public async Task GetEmployeesAsync_ReturnsOrderedAndPagedResults()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var ctx = new AppDbContext(options);
        ctx.Employees.AddRange(
            new Employee { EmpId = 2, EmpName = "B" },
            new Employee { EmpId = 1, EmpName = "A" },
            new Employee { EmpId = 3, EmpName = "C" });
        await ctx.SaveChangesAsync();

        var repo = new EmployeeRepository(ctx);

        var (items, totalCount) = await repo.GetEmployeesAsync(2, 1, CancellationToken.None);

        Assert.AreEqual(3, totalCount);
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual(2, items[0].EmpId);
    }
}
