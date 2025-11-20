using CamCare.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CamCare.Interfaces.Persistence
{
    public interface IKrdDbContext : IDisposable
    {
        DatabaseFacade Database { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbSet<Krd_Data> KrdData { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
