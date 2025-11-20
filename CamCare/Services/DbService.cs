using CamCare.Extensions;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace CamCare.Services
{
    public abstract class DbService<TEntity, TVm> : IDbService<TEntity, TVm>
        where TEntity : class, new()
        where TVm : class, new()
    {
        protected readonly IAppDbContextFactory _contextFactory;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;
        protected readonly NotificationService _notificationService;

        protected DbService(IAppDbContextFactory contextFactory, IMapper mapper, ILogger logger, NotificationService notificationService)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
            _logger = logger;
            _notificationService = notificationService;
        }

        protected void NotifyError(string summary, int duration = 5000, NotificationSeverity severity = NotificationSeverity.Error)
        {
            NotifyError(summary, null, duration, severity);
        }

        protected void NotifyError(string summary, string? message, int duration = 5000, NotificationSeverity severity = NotificationSeverity.Error)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = severity,
                Summary = summary,
                Detail = message,
                Duration = duration
            });
        }

        public virtual async Task<ServiceResponse<TVm>> GetByIdAsync(object id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entity = await context.Set<TEntity>().FindAsync(id);
                if (entity == null)
                    return ServiceResponse.Failure<TVm>("Nicht gefunden");
                var vm = _mapper.Map<TEntity, TVm>(entity);
                return ServiceResponse.Success(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GetByIdAsync");
                NotifyError("Fehler beim Abrufen des Datensatzes");
                return ServiceResponse.Failure<TVm>("Fehler beim Abrufen des Datensatzes", ex);
            }
        }

        public virtual async Task<ServiceResponse<List<TVm>>> GetAllAsync()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entities = await context.Set<TEntity>().ToListAsync();
                var vms = entities.Select(e => _mapper.Map<TEntity, TVm>(e)).ToList();
                return ServiceResponse.Success(vms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GetAllAsync");
                NotifyError("Fehler beim Abrufen der Daten");
                return ServiceResponse.Failure<List<TVm>>("Fehler beim Abrufen der Daten", ex);
            }
        }

        public virtual async Task<ServiceResponse<Paginated<TVm>>> GetAllAsync(LoadDataArgs args, Expression<Func<TEntity, bool>>? predicate = null)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var (totalCount, query) = context
                    .Set<TEntity>()
                    .AsNoTracking()
                    .AsQueryable()
                    .LoadByLoadDataArgs(args, predicate);

                var items = await query.ToListAsync();
                var vms = items.Select(e => _mapper.Map<TEntity, TVm>(e)).ToList();

                var paginated = new Paginated<TVm>
                {
                    Items = vms,
                    TotalCount = totalCount
                };
                return ServiceResponse.Success(paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GetAllAsync(LoadDataArgs)");
                NotifyError("Fehler beim Abrufen der Daten");
                return ServiceResponse.Failure<Paginated<TVm>>("Fehler beim Abrufen der Daten", ex);
            }
        }

        public virtual async Task<ServiceResponse<Paginated<TVm>>> GetAllAsync(LoadDataArgs args, params string[] includes)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var query = context
                    .Set<TEntity>()
                    .AsQueryable();

                if (typeof(IAuditableEntity).IsAssignableFrom(typeof(TEntity)))
                {
                    // Optional: Filter für archivierte Einträge
                    query = query.Where(e => !((IAuditableEntity)e).ArchivedAt.HasValue);
                }

                // Navigationen laden
                if (includes != null)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                // Filtering
                if (!string.IsNullOrEmpty(args.Filter))
                {
                    // Hinweis: Für produktiven Einsatz sollte ein dynamischer Filterbuilder verwendet werden
                }

                // Sorting
                if (!string.IsNullOrEmpty(args.OrderBy))
                {
                    query = query.OrderBy(args.OrderBy);
                }

                var totalCount = await query.CountAsync();

                // Paging
                if (args.Skip.HasValue)
                    query = query.Skip(args.Skip.Value);
                if (args.Top.HasValue)
                    query = query.Take(args.Top.Value);

                var items = await query.ToListAsync();
                var vms = items.Select(e => _mapper.Map<TEntity, TVm>(e)).ToList();

                var paginated = new Paginated<TVm>
                {
                    Items = vms,
                    TotalCount = totalCount
                };
                return ServiceResponse.Success(paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GetAllAsync(LoadDataArgs, includes)");
                NotifyError("Fehler beim Abrufen der Daten");
                return ServiceResponse.Failure<Paginated<TVm>>("Fehler beim Abrufen der Daten", ex);
            }
        }

        public virtual async Task<ServiceResponse<TVm>> CreateAsync(TVm vm)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entity = new TEntity();
                _mapper.Map(vm, entity);
                context.Set<TEntity>().Add(entity);
                await context.SaveChangesAsync();
                var resultVm = _mapper.Map<TEntity, TVm>(entity);
                return ServiceResponse.Success(resultVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in CreateAsync");
                NotifyError("Fehler beim Erstellen des Datensatzes");
                return ServiceResponse.Failure<TVm>("Fehler beim Erstellen des Datensatzes", ex);
            }
        }

        public virtual async Task<ServiceResponse<TVm>> UpdateAsync(object id, TVm vm)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entity = await context.Set<TEntity>().FindAsync(id);
                if (entity == null)
                    return ServiceResponse.Failure<TVm>("Nicht gefunden");
                _mapper.Map(vm, entity);
                await context.SaveChangesAsync();
                var resultVm = _mapper.Map<TEntity, TVm>(entity);
                return ServiceResponse.Success(resultVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in UpdateAsync");
                NotifyError("Fehler beim Aktualisieren des Datensatzes");
                return ServiceResponse.Failure<TVm>("Fehler beim Aktualisieren des Datensatzes", ex);
            }
        }

        public virtual async Task<ServiceResponse<bool>> DeleteAsync(object id, bool archive = true)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entity = await context.Set<TEntity>().FindAsync(id);
                if (entity == null)
                    return ServiceResponse.Failure<bool>("Nicht gefunden");

                if (entity is IAuditableEntity auditableEntity && archive)
                {
                    auditableEntity.ArchivedAt = DateTime.UtcNow;
                    context.Set<TEntity>().Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    context.Set<TEntity>().Remove(entity);
                }
                await context.SaveChangesAsync();
                return ServiceResponse.Success(true);
            }
            catch (DbUpdateException updateException)
            {
                string errorMessage = "Fehler beim Löschen des Datensatzes";
                if (updateException.InnerException is not null)
                {
                    if (updateException.InnerException.Message.Contains("conflicted with the REFERENCE constraint"))
                    {
                        errorMessage = "Löschen nicht möglich, da noch verknüpfte Daten existieren.";
                    }
                }
                _logger.LogError(updateException, "Fehler in DeleteAsync");
                NotifyError(errorMessage);
                return ServiceResponse.Failure<bool>(errorMessage, updateException);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in DeleteAsync");
                NotifyError("Fehler beim Löschen des Datensatzes");
                return ServiceResponse.Failure<bool>("Fehler beim Löschen des Datensatzes", ex);
            }
        }
    }
}
