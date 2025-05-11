using Microsoft.AspNetCore.Identity;
using PeopleRegistration.Application.DTOs;

namespace PeopleRegistration.Application.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterDto dto);
    Task<TokenResult?> LoginAsync(LoginDto dto);
}
