using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using PeopleRegistration.Domain.Entities;
using PeopleRegistration.Application.DTOs;
using PeopleRegistration.Domain.Interfaces;
using PeopleRegistration.Application.Interfaces;

namespace PeopleRegistration.Application.Services;

public class AuthService(IAuthRepository repo, IConfiguration config) : IAuthService
{
    private readonly IAuthRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));

    public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username))
            throw new ArgumentException("Username is required.", nameof(dto.Username));
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Password is required.", nameof(dto.Password));
        if (string.IsNullOrWhiteSpace(dto.Cpf))
            throw new ArgumentException("CPF is required.", nameof(dto.Cpf));
        if (dto.BirthDate >= DateTime.UtcNow)
            throw new ArgumentException("BirthDate must be in the past.", nameof(dto.BirthDate));

        var person = new Person(dto.Name, dto.Email, dto.BirthDate, dto.Cpf)
        {
            UserName = dto.Username
        };

        try
        {
            var result = await _repo.CreateUserAsync(person, dto.Password);
            return result;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Error creating user: " + ex.Message);
        }
    }

    public async Task<TokenResult?> LoginAsync(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("Username is required.", nameof(dto.Email));
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Password is required.", nameof(dto.Password));

        var user = await _repo.FindByEmailAsync(dto.Email);
        if (user is null)
            return null;

        var signin = await _repo.CheckPasswordSignInAsync(user, dto.Password);
        if (!signin.Succeeded)
            return null;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(2);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new TokenResult(
            Jwt: new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt: expires
        );
    }
}
