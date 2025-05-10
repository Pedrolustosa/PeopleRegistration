using Microsoft.AspNetCore.Identity;
using PeopleRegistration.Domain.Entities;
using PeopleRegistration.Domain.Interfaces;

namespace PeopleRegistration.Infrastructure.Repositories;

public class AuthRepository(UserManager<Person> users,
                      SignInManager<Person> signIn) : IAuthRepository
{
    private readonly UserManager<Person> _users = users;
    private readonly SignInManager<Person> _signIn = signIn;

    public Task<IdentityResult> CreateUserAsync(Person user, string password) =>
        _users.CreateAsync(user, password);

    public Task<Person?> FindByNameAsync(string userName) =>
        _users.FindByNameAsync(userName);

    public Task<SignInResult> CheckPasswordSignInAsync(Person user, string password) =>
        _signIn.CheckPasswordSignInAsync(user, password, false);
}
