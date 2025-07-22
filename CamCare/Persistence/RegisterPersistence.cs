using CamCare.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CamCare.Persistence
{
    public static class RegisterPersistence
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlite("Data Source=CamCare.db");
            });
            services.AddScoped<IAppDbContextFactory, AppDbContextFactory>();
            return services;
        }
    }
}
