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
            });
            services.AddScoped<IAppDbContextFactory, AppDbContextFactory>();


            var krdConnectionString = configuration.GetConnectionString("DefaultKrd");
            if (string.IsNullOrEmpty(krdConnectionString))
                throw new InvalidOperationException("Connection string 'Default' is not configured.");

            services.AddDbContextFactory<KrdDbContext>(options =>
            {
                options.UseSqlServer(krdConnectionString);
            });
            services.AddScoped<IKrdDbContextFactory, KrdDbContextFactory>();
            return services;
        }
    }
}
