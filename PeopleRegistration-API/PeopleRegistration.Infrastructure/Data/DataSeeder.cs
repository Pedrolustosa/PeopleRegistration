using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeopleRegistration.Domain.Entities;

namespace PeopleRegistration.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(this IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<Person>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            if (await context.People.AnyAsync())
                return;

            var sampleData = new List<(
                Guid Id,
                string UserName,
                string Email,
                string Password,
                string Name,
                DateTime BirthDate,
                string Cpf,
                string? Address)>
            {
                (Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    "alice",  "alice@example.com",  "Alice@123",  "Alice Silva",
                    new DateTime(1985,  3, 10), "52998224725", "Rua A, 100"),
                (Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    "bruno",  "bruno@example.com",  "Bruno@123",  "Bruno Souza",
                    new DateTime(1990,  7, 22), "11144477735", null),
                (Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    "carla",  "carla@example.com",  "Carla@123",  "Carla Pereira",
                    new DateTime(1978, 11,  5), "12345678909", "Travessa C, 300"),
                (Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    "daniel", "daniel@example.com", "Daniel@123", "Daniel Costa",
                    new DateTime(1995,  1, 30), "71199004472", null),
                (Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    "elena",  "elena@example.com",  "Elena@123",  "Elena Ramos",
                    new DateTime(1982,  6, 18), "73176135505", "Alameda E, 500"),
                (Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    "felipe", "felipe@example.com","Felipe@123","Felipe Oliveira",
                    new DateTime(1975, 12,  2), "80916880834", null),
            };

            foreach (var (id, userName, email, password, name, birthDate, cpf, address) in sampleData)
            {
                if (await userManager.FindByIdAsync(id.ToString()) is null)
                {
                    var person = new Person(name, email, birthDate, cpf)
                    {
                        Id       = id,
                        UserName = userName,
                        Email    = email
                    };
                    if (!string.IsNullOrWhiteSpace(address))
                        person.SetAddress(address);

                    var result = await userManager.CreateAsync(person, password);
                    if (!result.Succeeded)
                        foreach (var err in result.Errors)
                            Console.WriteLine($"Seed error for {userName}: {err.Description}");
                }
            }
        }
    }
}
