namespace CamCare.Interfaces.Persistence
{
    public interface IAppDbContextFactory
    {
        IAppDbContext CreateDbContext();
    }
}
