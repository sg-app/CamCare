using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CamCare.Persistence
{
    public class KrdDbContext(DbContextOptions<KrdDbContext> options) : DbContext(options), IKrdDbContext
    {
        public DbSet<Krd_Data> KrdData { get; set; }
    }
}
