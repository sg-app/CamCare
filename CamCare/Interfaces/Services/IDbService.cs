using CamCare.Models;
using Radzen;
using System.Linq.Expressions;

namespace CamCare.Interfaces.Services
{
    public interface IDbService<TEntity, TVm>
        where TEntity : class, new()
        where TVm : class, new()
    {
        Task<ServiceResponse<TVm>> GetByIdAsync(object id);
        Task<ServiceResponse<List<TVm>>> GetAllAsync();
        Task<ServiceResponse<Paginated<TVm>>> GetAllAsync(LoadDataArgs args, Expression<Func<TEntity, bool>>? predicate = null);
        Task<ServiceResponse<Paginated<TVm>>> GetAllAsync(LoadDataArgs args, params string[] includes);
        Task<ServiceResponse<TVm>> CreateAsync(TVm vm);
        Task<ServiceResponse<TVm>> UpdateAsync(object id, TVm vm);
        Task<ServiceResponse<bool>> DeleteAsync(object id, bool archive = true);
    }
}
