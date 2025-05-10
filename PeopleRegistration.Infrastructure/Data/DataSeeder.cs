using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PeopleRegistration.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace PeopleRegistration.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Person>>();

        await context.Database.EnsureCreatedAsync();

        if (await context.People.AnyAsync()) return;

        var sampleData = new List<(
            string UserName,
            string Email,
            string Password,
            string Name,
            DateTime BirthDate,
            string Cpf,
            string? Address)>
            {
                ("alice",  "alice@example.com",  "Alice@123",  "Alice Silva",    new DateTime(1985,3,10),  "52998224725", "Rua A, 100"),
                ("bruno",  "bruno@example.com",  "Bruno@123",  "Bruno Souza",    new DateTime(1990,7,22),  "11144477735", null),
                ("carla",  "carla@example.com",  "Carla@123",  "Carla Pereira",  new DateTime(1978,11,5),  "12345678909", "Travessa C, 300"),
                ("daniel", "daniel@example.com", "Daniel@123", "Daniel Costa",   new DateTime(1995,1,30), "71199004472", null),
                ("elena",  "elena@example.com",  "Elena@123",  "Elena Ramos",    new DateTime(1982,6,18),  "73176135505", "Alameda E, 500"),
                ("felipe", "felipe@example.com", "Felipe@123","Felipe Oliveira", new DateTime(1975,12,2), "80916880834", null)
            };


        foreach (var (userName, email, password, name, birthDate, cpf, address) in sampleData)
        {
            if (await userManager.FindByNameAsync(userName) is null)
            {
                var person = new Person(name, email, birthDate, cpf);
                if (!string.IsNullOrWhiteSpace(address))
                    person.SetAddress(address);

                var result = await userManager.CreateAsync(person, password);
                if (result.Succeeded) ;
                else
                {
                    foreach (var error in result.Errors)
                        Console.WriteLine($"Error creating user {userName}: {error.Description}");
                }
            }
        }
    }
}