using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PeopleRegistration.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace PeopleRegistration.Infrastructure.IoC;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection")
                   ?? "Data Source=people.db";
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseSqlite(conn));
        return services;
    }
}
