using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using CamCare.Options;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Radzen;
using System.Linq.Dynamic.Core;

namespace CamCare.Services
{
    public class DataStoreService : DbService<DataStore, DataStoreVm>, IDataStoreService
    {
        private static readonly string[] SupportedContentTypes = ["application/pdf"];
        private readonly IObjectStorageService _objectStorageService;
        private readonly ObjectStorageOptions _objectStorageOptions;
        private readonly NavigationManager _navigationManager;

        public DataStoreService(
            IAppDbContextFactory contextFactory,
            IMapper mapper,
            ILogger<DataStoreService> logger,
            NotificationService notificationService,
            IObjectStorageService objectStorageService,
            IOptions<ObjectStorageOptions> objectStorageOptions,
            NavigationManager navigationManager)
            : base(contextFactory, mapper, logger, notificationService)
        {
            _objectStorageService = objectStorageService;
            _objectStorageOptions = objectStorageOptions.Value;
            _navigationManager = navigationManager;
        }

        public override async Task<ServiceResponse<DataStoreVm>> CreateAsync(DataStoreVm vm)
        {
            try
            {
                if (vm.Data is null || vm.Data.Length == 0)
                    return ServiceResponse.Failure<DataStoreVm>("Keine Datei zum Speichern vorhanden.");

                if (vm.Data.LongLength > _objectStorageOptions.MaxFileSizeBytes)
                    return ServiceResponse.Failure<DataStoreVm>("Datei ist zu groß. Maximal 10MB erlaubt.");

                if (!IsSupportedContentType(vm.Type))
                    return ServiceResponse.Failure<DataStoreVm>("Nur PDF- und Bilddateien sind erlaubt.");

                var objectKey = BuildObjectKey(vm.RepairOrderId, vm.Filename);
                var metadata = BuildObjectMetadata(vm.RepairOrderId, vm.Filename);

                await using (var uploadStream = new MemoryStream(vm.Data, writable: false))
                {
                    await _objectStorageService.UploadAsync(objectKey, uploadStream, vm.Type, metadata);
                }

                using var context = _contextFactory.CreateDbContext();

                var entity = _mapper.Map<DataStoreVm, DataStore>(vm);
                entity.ObjectKey = objectKey;
                entity.SizeBytes = vm.Data.LongLength;
                entity.Data = null;

                context.DataStores.Add(entity);
                await context.SaveChangesAsync();

                var resultVm = _mapper.Map<DataStore, DataStoreVm>(entity);
                resultVm.Data = null;
                resultVm.DownloadUrl = CreateInternalReadUrl(resultVm.Id);

                return ServiceResponse.Success(resultVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in DataStore CreateAsync für RepairOrder {RepairOrderId}", vm.RepairOrderId);
                NotifyError("Fehler beim Speichern der Datei");
                return ServiceResponse.Failure<DataStoreVm>("Fehler beim Speichern der Datei", ex);
            }
        }


        public async Task<ServiceResponse<List<DataStoreVm>>> GetFromRepairOrderAsync(int repairOrderId)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var items = await context.DataStores
                    .Where(ds => ds.RepairOrderId == repairOrderId)
                    .ToListAsync();

                if (items.Count > 0)
                {
                    var orphanedItems = new List<DataStore>();

                    foreach (var item in items)
                    {
                        if (string.IsNullOrWhiteSpace(item.ObjectKey))
                            continue;

                        var exists = await _objectStorageService.ExistsAsync(item.ObjectKey);
                        if (!exists)
                        {
                            orphanedItems.Add(item);
                        }
                    }

                    if (orphanedItems.Count > 0)
                    {
                        context.DataStores.RemoveRange(orphanedItems);
                        await context.SaveChangesAsync();

                        var orphanedIds = orphanedItems.Select(x => x.Id).ToHashSet();
                        items = items.Where(x => !orphanedIds.Contains(x.Id)).ToList();

                        _logger.LogInformation(
                            "{Count} verwaiste DataStore-Datensätze für RepairOrder {RepairOrderId} wurden beim Laden entfernt.",
                            orphanedItems.Count,
                            repairOrderId);
                    }
                }

                var vms = items.Select(_mapper.Map<DataStore, DataStoreVm>).ToList();

                foreach (var vm in vms)
                {
                    vm.DownloadUrl = CreateInternalReadUrl(vm.Id);
                    if (!string.IsNullOrWhiteSpace(vm.ObjectKey))
                    {
                        vm.Data = null;
                    }
                }

                return ServiceResponse.Success(vms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GetFromRepairOrderAsync {id}", repairOrderId);
                NotifyError("Fehler beim Abrufen der Daten");
                return ServiceResponse.Failure<List<DataStoreVm>>("Fehler beim Abrufen der Daten", ex);
            }
        }

        public override async Task<ServiceResponse<DataStoreVm>> GetByIdAsync(object id)
        {
            var response = await base.GetByIdAsync(id);
            if (!response.Success || response.Data is null)
                return response;

            response.Data.DownloadUrl = CreateInternalReadUrl(response.Data.Id);
            if (!string.IsNullOrWhiteSpace(response.Data.ObjectKey))
            {
                response.Data.Data = null;
            }

            return response;
        }

        public override async Task<ServiceResponse<bool>> DeleteAsync(object id, bool archive = true)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entity = await context.DataStores.FindAsync(id);
                if (entity == null)
                    return ServiceResponse.Failure<bool>("Nicht gefunden");

                if (!string.IsNullOrWhiteSpace(entity.ObjectKey))
                {
                    await _objectStorageService.DeleteAsync(entity.ObjectKey);
                }

                if (archive)
                {
                    entity.ArchivedAt = DateTime.UtcNow;
                }
                else
                {
                    context.DataStores.Remove(entity);
                }

                await context.SaveChangesAsync();
                return ServiceResponse.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in DataStore DeleteAsync für ID {Id}", id);
                NotifyError("Fehler beim Löschen der Datei");
                return ServiceResponse.Failure<bool>("Fehler beim Löschen der Datei", ex);
            }
        }

        public async Task<ServiceResponse<DataStoreFileVm>> GetFileAsync(int dataStoreId)
        {
            using var context = _contextFactory.CreateDbContext();
            DataStore? item = null;

            try
            {
                item = await context.DataStores
                    .FirstOrDefaultAsync(ds => ds.Id == dataStoreId);

                if (item is null)
                    return ServiceResponse.Failure<DataStoreFileVm>("Datei nicht gefunden.");

                byte[] content;
                if (!string.IsNullOrWhiteSpace(item.ObjectKey))
                {
                    await using var stream = await _objectStorageService.DownloadAsync(item.ObjectKey);
                    using var memory = new MemoryStream();
                    await stream.CopyToAsync(memory);
                    content = memory.ToArray();
                }
                else if (item.Data is not null && item.Data.Length > 0)
                {
                    content = item.Data;
                }
                else
                {
                    return ServiceResponse.Failure<DataStoreFileVm>("Dateiinhalt ist leer.");
                }

                return ServiceResponse.Success(new DataStoreFileVm
                {
                    Filename = item.Filename,
                    ContentType = string.IsNullOrWhiteSpace(item.Type) ? "application/octet-stream" : item.Type,
                    Content = content
                });
            }
            catch (NoSuchKeyException ex)
            {
                _logger.LogWarning(ex, "Objekt mit Key {ObjectKey} für DataStore ID {Id} wurde im ObjectStorage nicht gefunden. Entferne verwaisten Datensatz.", item?.ObjectKey, dataStoreId);

                if (item is not null)
                {
                    context.DataStores.Remove(item);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Verwaister DataStore-Datensatz mit ID {Id} wurde gelöscht.", dataStoreId);
                }

                return ServiceResponse.Failure<DataStoreFileVm>("Datei wurde im ObjectStorage nicht gefunden. Der verwaiste Datenbankeintrag wurde entfernt.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GetFileAsync für DataStore ID {Id}", dataStoreId);
                return ServiceResponse.Failure<DataStoreFileVm>("Fehler beim Abrufen der Datei.", ex);
            }
        }

        private bool IsSupportedContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
                return false;

            return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                   || SupportedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);
        }

        private string BuildObjectKey(int repairOrderId, string filename)
        {
            var extension = Path.GetExtension(filename);
            var baseName = Path.GetFileNameWithoutExtension(filename);
            var safeBaseName = SanitizeObjectName(baseName);
            var shortGuid = Guid.NewGuid().ToString("N")[..8];

            return $"repair-orders/{repairOrderId}/{safeBaseName}-{shortGuid}{extension}";
        }

        private static string SanitizeObjectName(string? rawName)
        {
            const string fallbackName = "file";
            const int maxLength = 64;

            if (string.IsNullOrWhiteSpace(rawName))
                return fallbackName;

            var invalidChars = Path.GetInvalidFileNameChars();
            var buffer = new char[rawName.Length];
            var length = 0;

            foreach (var ch in rawName.Trim())
            {
                var normalized = ch == ' ' || ch == '_' ? '-' : ch;

                if (char.IsControl(normalized) || Array.IndexOf(invalidChars, normalized) >= 0)
                    normalized = '-';

                if (normalized == '-' && length > 0 && buffer[length - 1] == '-')
                    continue;

                buffer[length++] = normalized;
            }

            var sanitized = new string(buffer, 0, length).Trim('-', '.');

            if (sanitized.Length > maxLength)
                sanitized = sanitized[..maxLength].Trim('-', '.');

            return string.IsNullOrWhiteSpace(sanitized) ? fallbackName : sanitized;
        }

        private string CreateInternalReadUrl(int dataStoreId)
        {
            return _navigationManager.ToAbsoluteUri($"/api/datastores/{dataStoreId}/file").ToString();
        }

        private static IReadOnlyDictionary<string, string> BuildObjectMetadata(int repairOrderId, string filename)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["original-filename"] = string.IsNullOrWhiteSpace(filename) ? "unknown" : filename,
                ["repair-order-id"] = repairOrderId.ToString(),
                ["uploaded-at-utc"] = DateTime.UtcNow.ToString("O")
            };
        }

        private async Task<string?> CreateReadUrlAsync(string? objectKey)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                return null;

            return await _objectStorageService.GetReadUrlAsync(
                objectKey,
                TimeSpan.FromMinutes(Math.Max(1, _objectStorageOptions.PresignedUrlLifetimeMinutes)));
        }
    }
}
