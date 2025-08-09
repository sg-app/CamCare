using CamCare.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CamCare.Persistence
{
    public class KrdDbContextFactory(IDbContextFactory<KrdDbContext> dbContextFactory) : IKrdDbContextFactory
    {
        public IKrdDbContext CreateDbContext()
            => dbContextFactory.CreateDbContext();
    }
}
