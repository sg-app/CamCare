namespace CamCare.Models;

public class EmployeeVm
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string DisplayName
        => $"{FirstName} {LastName}";

    public override string ToString()
        => $"[{Id}] {DisplayName}";
}
