using PeopleRegistration.Domain.Interfaces;
using PeopleRegistration.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using PeopleRegistration.Application.Factories;
using PeopleRegistration.Application.Interfaces;
using PeopleRegistration.Infrastructure.Factories;
using PeopleRegistration.Infrastructure.Repositories;

namespace PeopleRegistration.Infrastructure.IoC;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IPersonFactory, PersonFactory>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
