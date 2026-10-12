using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using CamCare.Security;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace CamCare.Services
{
    public class UserGroupService(
        IAppDbContextFactory contextFactory,
        ILogger<UserGroupService> logger,
        NotificationService notificationService) : IUserGroupService
    {
        private readonly SemaphoreSlim _initializationLock = new(1, 1);
        private volatile bool _isInitialized;

        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            await _initializationLock.WaitAsync();
            try
            {
                if (_isInitialized)
                    return;

                using var context = contextFactory.CreateDbContext();
                await SeedMissingGroupsAsync(context);

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Initialisierung der Benutzergruppen ist fehlgeschlagen.");
                throw;
            }
            finally
            {
                _initializationLock.Release();
            }
        }

        /// <summary>
        /// Legt die bekannten Benutzergruppen mit ihren Standard-Passwörtern an, falls sie fehlen.
        /// Idempotent — vorhandene Gruppen (und damit ggf. bereits geänderte Passwörter) bleiben unverändert.
        /// </summary>
        private async Task SeedMissingGroupsAsync(IAppDbContext context)
        {
            var existingNames = await context.Set<UserGroup>()
                .AsNoTracking()
                .Select(g => g.Name)
                .ToListAsync();

            var added = false;
            foreach (var group in UserGroups.All)
            {
                if (existingNames.Contains(group.Name, StringComparer.OrdinalIgnoreCase))
                    continue;

                context.Set<UserGroup>().Add(new UserGroup
                {
                    Name = group.Name,
                    PasswordHash = PasswordHasher.Hash(group.DefaultPassword)
                });
                added = true;
                logger.LogInformation("Benutzergruppe '{GroupName}' mit Standard-Passwort angelegt.", group.Name);
            }

            if (added)
                await context.SaveChangesAsync();
        }

        public async Task<UserGroup?> ValidateAsync(string groupName, string password)
        {
            if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(password))
                return null;

            try
            {
                using var context = contextFactory.CreateDbContext();
                var normalizedGroupName = groupName.Trim();

                var userGroup = await FindGroupAsync(context, normalizedGroupName);
                if (userGroup is null)
                {
                    // Selbst-Heilung: Falls das Seeding beim Start nicht lief (z. B. frisch
                    // aufgesetzte Datenbank), werden die Standard-Benutzergruppen nachgezogen.
                    await SeedMissingGroupsAsync(context);
                    userGroup = await FindGroupAsync(context, normalizedGroupName);

                    if (userGroup is null)
                    {
                        logger.LogWarning("Anmeldeversuch mit unbekannter Benutzergruppe '{GroupName}'.", normalizedGroupName);
                        return null;
                    }
                }

                if (!PasswordHasher.Verify(password, userGroup.PasswordHash))
                {
                    logger.LogWarning("Anmeldeversuch mit falschem Passwort für Gruppe '{GroupName}'.", userGroup.Name);
                    return null;
                }

                return userGroup;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler bei der Passwortprüfung für Gruppe '{GroupName}'.", groupName);
                return null;
            }
        }

        private static Task<UserGroup?> FindGroupAsync(IAppDbContext context, string groupName)
            => context.Set<UserGroup>()
                .AsNoTracking()
                .FirstOrDefaultAsync(g =>
                    g.Name.ToLower() == groupName.ToLower());

        public async Task<ServiceResponse<List<UserGroupVm>>> GetAllAsync()
        {
            try
            {
                using var context = contextFactory.CreateDbContext();
                var groups = await context.Set<UserGroup>()
                    .AsNoTracking()
                    .OrderBy(g => g.Name)
                    .Select(g => new UserGroupVm
                    {
                        Id = g.Id,
                        Name = g.Name
                    })
                    .ToListAsync();

                return ServiceResponse.Success(groups);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Laden der Benutzergruppen.");
                NotifyError();
                return ServiceResponse.Failure<List<UserGroupVm>>("Fehler beim Laden der Benutzergruppen.", ex);
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePasswordAsync(int id, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newPassword))
                    return ServiceResponse.Failure<bool>("Das Passwort darf nicht leer sein.");

                using var context = contextFactory.CreateDbContext();
                var userGroup = await context.Set<UserGroup>().FindAsync(id);
                if (userGroup is null)
                    return ServiceResponse.Failure<bool>("Benutzergruppe nicht gefunden.");

                userGroup.PasswordHash = PasswordHasher.Hash(newPassword);
                await context.SaveChangesAsync();

                logger.LogInformation("Passwort der Benutzergruppe '{GroupName}' wurde geändert.", userGroup.Name);
                NotificationServiceSuccess(userGroup.Name);
                return ServiceResponse.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Ändern des Passworts.");
                NotifyError();
                return ServiceResponse.Failure<bool>("Fehler beim Ändern des Passworts.", ex);
            }
        }

        private void NotifyError()
            => notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Fehler beim Ändern des Passworts",
                Duration = 5000
            });

        private void NotificationServiceSuccess(string groupName)
            => notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = $"Passwort für '{groupName}' wurde geändert.",
                Duration = 5000
            });
    }
}
