using CamCare.Models;
using CamCare.Models.Amicron;
using Radzen;
using System.Linq.Expressions;

namespace CamCare.Interfaces.Services
{
    public interface IAmicronDataService
    {
        Task<ServiceResponse<Paginated<Adressen>>> GetAllAddressAsync(LoadDataArgs args, AdressenFilter? filter = null);
        Task<ServiceResponse<Adressen>> GetAddressByCustomerIdAsync(int customerId);
        Task<ServiceResponse<Paginated<Serials>>> GetSerialsFromCustomerIdAsync(LoadDataArgs args, int customerId);
        Task<ServiceResponse<Paginated<Artikel>>> GetArticleAsync(LoadDataArgs args, ArtikelFilter? filter = null);
    }
}