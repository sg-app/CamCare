using CamCare.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CamCare.Persistence
{
    public static class RegisterPersistence
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'Default' is not configured.");
            
            services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                //options.UseSqlite(connectionString);
            });
            services.AddScoped<IAppDbContextFactory, AppDbContextFactory>();
            return services;
        }
    }
}
