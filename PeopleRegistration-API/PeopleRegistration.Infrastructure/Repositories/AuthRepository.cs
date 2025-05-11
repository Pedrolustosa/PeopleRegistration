using Microsoft.AspNetCore.Identity;
using PeopleRegistration.Domain.Entities;
using PeopleRegistration.Domain.Interfaces;

namespace PeopleRegistration.Infrastructure.Repositories;

public class AuthRepository(
    UserManager<Person> users,
    SignInManager<Person> signIn) : IAuthRepository
{
    private readonly UserManager<Person> _users = users  ?? throw new ArgumentNullException(nameof(users));
    private readonly SignInManager<Person> _signIn = signIn ?? throw new ArgumentNullException(nameof(signIn));

    public async Task<IdentityResult> CreateUserAsync(Person user, string password)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        try
        {
            var result = await _users.CreateAsync(user, password);
            return result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error creating user account: " + ex.Message, ex);
        }
    }

    public async Task<Person?> FindByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("email is required.", nameof(email));

        try
        {
            return await _users.FindByEmailAsync(email);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error finding user: " + ex.Message, ex);
        }
    }

    public async Task<SignInResult> CheckPasswordSignInAsync(Person user, string password)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        try
        {
            return await _signIn.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error verifying user password: " + ex.Message, ex);
        }
    }
}
