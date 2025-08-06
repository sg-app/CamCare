using CamCare.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CamCare.Interfaces.Persistence
{
    public interface IAppDbContext : IDisposable
    {
        DatabaseFacade Database { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbSet<Customer> Customers { get; set; }
        DbSet<Camera> Cameras { get; set; }
        DbSet<CameraType> CameraTypes { get; set; }
        DbSet<Address> Addresses { get; set; }
        DbSet<Defective> Defectives { get; set; }
        DbSet<RepairOrderStatus> RepairOrderStatuses { get; set; }
        DbSet<RepairOrder> RepairOrders { get; set; }
        DbSet<RepairPosition> RepairPositions { get; set; }
        DbSet<LogisticProvider> LogisticProviders { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
