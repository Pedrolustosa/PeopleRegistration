using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PeopleRegistration.Infrastructure.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPeopleRegistrationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddDatabase(configuration)
            .AddIdentityAuthentication(configuration)
            .AddAuthorizationPolicies()
            .AddApiVersioning()
            .AddSwaggerDocumentation()
            .AddApplicationServices();
    }
}
