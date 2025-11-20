namespace CamCare.Interfaces.Persistence
{
    public interface IKrdDbContextFactory
    {
        IKrdDbContext CreateDbContext();
    }
}
