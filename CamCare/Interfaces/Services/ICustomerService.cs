using CamCare.Domain;
using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    public interface ICustomerService : IDbService<Customer, CustomerVm>
    {

    }
}
