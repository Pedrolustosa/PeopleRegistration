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
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseInMemoryDatabase("PeopleDb"));

        return services;
    }
}