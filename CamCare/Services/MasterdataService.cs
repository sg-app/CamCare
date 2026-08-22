using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace CamCare.Services
{
    public class MasterdataService(
        IServiceProvider serviceProvider,
        ILogger<MasterdataService> logger,
        IConfiguration configuration,
        IMemoryCache memoryCache) : IMasterdataService
    {
        private int MaxDegreeOfParallelism => configuration.GetValue("MaxDegreeOfParallelismLoadMasterData", 10);
        private TimeSpan CacheTtl => TimeSpan.FromMinutes(configuration.GetValue("MasterdataCacheTtlMinutes", 60));

        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly Dictionary<Type, Func<Task>> _entityLoaderMap = [];
        private readonly ConcurrentDictionary<Type, object> _lastKnownData = [];
        private readonly ConcurrentDictionary<Type, byte> _backgroundReloadInProgress = [];
        private readonly SemaphoreSlim _initializationLock = new(1, 1);
        private volatile bool _isInitialized;

        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            await _initializationLock.WaitAsync();
            if (_isInitialized)
            {
                _initializationLock.Release();
                return;
            }

            try
            {
                RegisterLoaders();

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
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Initialisierung der Stammdaten ist fehlgeschlagen.");
                throw;
            }
            finally
            {
                _initializationLock.Release();
            }
        }

        public List<T> Get<T>() where T : class
        {
            Type keyType = typeof(T);
            RegisterLoaders();

            if (_memoryCache.TryGetValue(keyType, out List<T>? data) && data is not null)
            {
                return [.. data];
            }

            if (_lastKnownData.TryGetValue(keyType, out var staleData) && staleData is List<T> staleList)
            {
                TryTriggerBackgroundReload<T>();
                return [.. staleList];
            }

            // If the entry has expired or was never loaded, trigger a best-effort background refresh.
            TryTriggerBackgroundReload<T>();
            return [];
        }

        public async Task ReloadAsync<T>() where T : class
        {
            RegisterLoaders();

            if (!_entityLoaderMap.TryGetValue(typeof(T), out var loader))
                throw new InvalidOperationException($"Es ist kein Loader für den Typ '{typeof(T).Name}' registriert.");

            await loader();
        }

        public async Task<bool> TryReloadAsync<T>() where T : class
        {
            RegisterLoaders();

            if (!_entityLoaderMap.TryGetValue(typeof(T), out var loader))
                return false;

            await loader();
            return true;
        }

        private void RegisterLoaders()
        {
            if (_entityLoaderMap.Count > 0)
                return;

            Register<Employee, EmployeeVm>();
            Register<LogisticProvider, LogisticProviderVm>();
            Register<RepairOrderStatus, RepairOrderStatusVm>();
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
                logger.LogInformation("Stammdaten geladen: {Entity} ({Count} Eintraege)", typeof(TEntity).Name, viewModels.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Laden der Stammdaten für {Entity}", typeof(TEntity).Name);
                throw;
            }
        }

        private void Set<T>(List<T> value) where T : class
        {
            Type keyType = typeof(T);
            var snapshot = value.ToList();
            _memoryCache.Set(keyType, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheTtl
            });
            _lastKnownData[keyType] = snapshot;
        }

        private void TryTriggerBackgroundReload<T>() where T : class
        {
            Type keyType = typeof(T);

            if (_backgroundReloadInProgress.TryAdd(keyType, 0) is false)
                return;

            _ = Task.Run(async () =>
            {
                try
                {
                    await TryReloadAsync<T>();
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Background-Reload fuer Stammdaten {Type} fehlgeschlagen.", keyType.Name);
                }
                finally
                {
                    _backgroundReloadInProgress.TryRemove(keyType, out _);
                }
            });
        }

    }
}
