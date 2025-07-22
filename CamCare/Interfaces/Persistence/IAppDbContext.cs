using CamCare.Domain;
using Microsoft.EntityFrameworkCore;

namespace CamCare.Interfaces.Persistence
{
    public interface IAppDbContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbSet<Customer> Customers { get; set; }
        DbSet<Address> Addresses { get; set; }
        DbSet<AddressType> AddressTypes { get; set; }
        DbSet<RepairOrderStatus> RepairOrderStatuses { get; set; }
        DbSet<RepairOrder> RepairOrders { get; set; }
        DbSet<RepairPosition> RepairPositions { get; set; }
        DbSet<RepairOrderRepairPosition> RepairOrderRepairPositions { get; set; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
