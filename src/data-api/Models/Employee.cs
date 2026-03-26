using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataApi.Models;

[Table("employees", Schema = "public")]
public class Employee
{
    [Key]
    [Column("emp_id")]
    public int EmpId { get; set; }

    [Column("emp_name")]
    public string? EmpName { get; set; }
}
