using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace CamCare.Services
{
    public class MasterdataService(IServiceProvider serviceProvider, ILogger<MasterdataService> logger, IConfiguration configuration) : IMasterdataService
    {
        private int MaxDegreeOfParallelism => configuration.GetValue("MaxDegreeOfParallelismLoadMasterData", 10);
        private readonly Dictionary<Type, Func<Task>> _entityLoaderMap = [];
        private readonly ConcurrentDictionary<Type, object> _data = [];

        public async Task InitializeAsync()
        {
            Register<Employee, EmployeeVm>();
            Register<LogisticProvider, LogisticProviderVm>();
            Register<RepairOrderStatus, RepairOrderStatusVm>();

            var semaphore = new SemaphoreSlim(MaxDegreeOfParallelism);
            var tasks = _entityLoaderMap.Values.Select(async loader =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await loader();
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToList();

            await Task.WhenAll(tasks);
        }

        public List<T> Get<T>() where T : class
        {
            if (_data.TryGetValue(typeof(T), out var data))
                return data as List<T> ?? [];

            return [];
        }
        public async Task ReloadAsync<T>() where T : class
        {
            var task = _entityLoaderMap[typeof(T)]();
            await task;
        }

        private void Register<TEntity, TViewModel>()
            where TEntity : class
            where TViewModel : class, new()
        {
            _entityLoaderMap[typeof(TViewModel)] = () => LoadAsync<TEntity, TViewModel>();
        }

        private async Task LoadAsync<TEntity, TViewModel>()
            where TEntity : class
            where TViewModel : class, new()
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContextFactory = scope.ServiceProvider.GetRequiredService<IAppDbContextFactory>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                using var dbContext = dbContextFactory.CreateDbContext();
                var data = await dbContext.Set<TEntity>()
                    .AsNoTracking()
                    .ToListAsync();
                var viewModels = data.Select(mapper.Map<TEntity, TViewModel>).ToList();

                Set(viewModels);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Laden der Stammdaten für {Entity}", typeof(TEntity).Name);
            }
        }

        private void Set<T>(List<T> value) where T : class
        {
            Type keyType = typeof(T);
            _data.AddOrUpdate(keyType, value, (type, oldValue) => value);
        }

    }
}
