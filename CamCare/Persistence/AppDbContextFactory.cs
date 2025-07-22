using CamCare.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CamCare.Persistence
{
    public class AppDbContextFactory(IDbContextFactory<AppDbContext> dbContextFactory) : IAppDbContextFactory
    {
        public IAppDbContext CreateDbContext()
            => dbContextFactory.CreateDbContext();
    }
}
