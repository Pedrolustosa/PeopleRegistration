using Microsoft.AspNetCore.Identity;
using PeopleRegistration.Domain.Entities;

namespace PeopleRegistration.Domain.Interfaces;

public interface IAuthRepository
{
    Task<IdentityResult> CreateUserAsync(Person user, string password);
    Task<Person?> FindByEmailAsync(string email);
    Task<SignInResult> CheckPasswordSignInAsync(Person user, string password);
}
