using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PeopleRegistration.Application.DTOs;
using PeopleRegistration.Application.Common;
using PeopleRegistration.Application.Interfaces;

namespace PeopleRegistration.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController(IAuthService auth) : ControllerBase
{
    private readonly IAuthService _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(
                ApiResponse<EmptyClass>.Fail(errors, ApiMessages.ValidationFailed)
            );
        }

        var result = await _auth.RegisterAsync(dto);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(
                ApiResponse<EmptyClass>.Fail(errors, ApiMessages.RegistrationError)
            );
        }

        return Created(
            string.Empty,
            ApiResponse<EmptyClass>.Ok(null!, ApiMessages.RegistrationSuccess)
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(
                ApiResponse<EmptyClass>.Fail(errors, ApiMessages.ValidationFailed)
            );
        }

        var token = await _auth.LoginAsync(dto);
        if (token is null)
        {
            return Unauthorized(
                ApiResponse<EmptyClass>.Fail(
                    [ApiMessages.InvalidCredentials]
                )
            );
        }

        return Ok(
            ApiResponse<TokenResult>.Ok(token, ApiMessages.LoginSuccess)
        );
    }
}
