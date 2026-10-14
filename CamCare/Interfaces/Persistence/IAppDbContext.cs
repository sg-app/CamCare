using CamCare.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CamCare.Interfaces.Persistence
{
    public interface IAppDbContext : IDisposable
    {
        DatabaseFacade Database { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbSet<Defective> Defectives { get; set; }
        DbSet<Work> Works { get; set; }
        DbSet<IncludedComponent> IncludedComponents { get; set; }
        DbSet<RepairOrderStatus> RepairOrderStatuses { get; set; }
        DbSet<RepairOrder> RepairOrders { get; set; }
        DbSet<RepairPosition> RepairPositions { get; set; }
        DbSet<LogisticProvider> LogisticProviders { get; set; }
        DbSet<RepairOrderStatusHistory> RepairOrderStatusHistories { get; set; }
        DbSet<DataStore> DataStores { get; set; }
        DbSet<UserGroup> UserGroups { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
