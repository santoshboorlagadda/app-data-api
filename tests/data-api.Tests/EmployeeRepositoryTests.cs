using DataApi.Data;
using DataApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataApi.Tests;

[TestClass]
public sealed class EmployeeRepositoryTests
{
    [TestMethod]
    public async Task CountEmployeesAsync_NoFilters_ReturnsCount()
    {
        var options = new DbContextOptionsBuilder<MysampleContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var db = new MysampleContext(options);
        db.Employees.AddRange(
            new DataApi.Models.Employee { EmpId = 1, EmpName = "Alice" },
            new DataApi.Models.Employee { EmpId = 2, EmpName = "Bob" });
        await db.SaveChangesAsync();

        var repo = new EmployeeRepository(db);

        var count = await repo.CountEmployeesAsync(null, null, CancellationToken.None);

        Assert.AreEqual(2, count);
    }
}
