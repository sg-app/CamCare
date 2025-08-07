using CamCare.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CamCare.Persistence
{
    public static class RegisterPersistence
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("Default"));
            });
            services.AddScoped<IAppDbContextFactory, AppDbContextFactory>();
            return services;
        }
    }
}
