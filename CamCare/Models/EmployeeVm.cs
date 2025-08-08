using System.ComponentModel.DataAnnotations;

namespace CamCare.Models;

public class EmployeeVm
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
