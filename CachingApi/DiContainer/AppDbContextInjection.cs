using CachingApi.Database;
using Microsoft.EntityFrameworkCore;

namespace CachingApi.DiContainer;

public static class AppDbContextInjection
{
    public static IServiceCollection AppDbContextConfig(this IServiceCollection Services, IConfiguration configuration)
    {
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 32));
        var connString = configuration.GetConnectionString("Dapper");
        Services.AddDbContext<ApplicationDbContext>(
            options => options.UseMySql(connString, ServerVersion.AutoDetect(connString)));

        return Services;
    }
}