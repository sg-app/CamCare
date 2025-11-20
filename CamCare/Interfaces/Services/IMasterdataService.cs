namespace CamCare.Interfaces.Services
{
    public interface IMasterdataService
    {
        List<T> Get<T>() where T : class;
        Task InitializeAsync();
        Task ReloadAsync<T>() where T : class;
    }
}