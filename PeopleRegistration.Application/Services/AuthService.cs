using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using PeopleRegistration.Domain.Entities;
using Microsoft.Extensions.Configuration;
using PeopleRegistration.Application.DTOs;
using PeopleRegistration.Domain.Interfaces;
using PeopleRegistration.Application.Interfaces;

namespace PeopleRegistration.Application.Services;

public class AuthService(IAuthRepository repo, IConfiguration config) : IAuthService
{
    private readonly IAuthRepository _repo = repo;
    private readonly IConfiguration _config = config;

    public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
    {
        var person = new Person(dto.Name, dto.Email, dto.BirthDate, dto.Cpf)
        {
            UserName = dto.Username
        };

        return await _repo.CreateUserAsync(person, dto.Password);
    }

    public async Task<TokenResult?> LoginAsync(LoginDto dto)
    {
        var user = await _repo.FindByNameAsync(dto.Email);
        if (user is null) return null;

        var signin = await _repo.CheckPasswordSignInAsync(user, dto.Password);
        if (!signin.Succeeded) return null;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var exp = DateTime.UtcNow.AddHours(2);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: exp,
            signingCredentials: creds
        );

        return new TokenResult(
            Jwt: new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt: exp
        );
    }
}
