using DataApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DataApi.Data;

public partial class MysampleContext : DbContext
{
    public MysampleContext()
    {
    }

    public MysampleContext(DbContextOptions<MysampleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees", "public");

            entity.HasKey(e => e.EmpId).HasName("employees_pkey");

            entity.Property(e => e.EmpId).HasColumnName("emp_id");
            entity.Property(e => e.EmpName).HasMaxLength(255).HasColumnName("emp_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
