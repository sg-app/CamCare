using CamCare.Domain;
using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    public interface IEmployeeService : IDbService<Employee, EmployeeVm>
    {
    }
}
