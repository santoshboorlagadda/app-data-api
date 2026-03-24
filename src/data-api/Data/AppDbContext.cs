using DataApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DataApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmpId);
            entity.Property(e => e.EmpId).UseIdentityAlwaysColumn();
        });

        base.OnModelCreating(modelBuilder);
    }
}
