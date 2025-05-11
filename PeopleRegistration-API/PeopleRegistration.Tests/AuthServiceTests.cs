using Moq;
using Microsoft.AspNetCore.Identity;
using PeopleRegistration.Domain.Entities;
using Microsoft.Extensions.Configuration;
using PeopleRegistration.Application.DTOs;
using PeopleRegistration.Domain.Interfaces;
using PeopleRegistration.Application.Services;

namespace PeopleRegistration.Tests;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _repoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _repoMock   = new(MockBehavior.Strict);
        _configMock = new(MockBehavior.Strict);

        _configMock.Setup(c => c["Jwt:Key"]).Returns("uma-chave-muito-secreta-e-longa-32chars!");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("PeopleApi");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("PeopleApiUsers");

        _service = new AuthService(_repoMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsSucceeded_OnValidDto()
    {
        var dto = new RegisterDto(
            Name: "Alice Silva",
            Username: "alice",
            Email: "alice@example.com",
            BirthDate: new DateTime(1985, 3, 10),
            Cpf: "52998224725",
            Password: "Alice@123"
        );

        var identityResult = IdentityResult.Success;
        _repoMock.Setup(r => r.CreateUserAsync(
                It.Is<Person>(p => p.UserName == dto.Username && p.Email == dto.Email && p.Cpf == dto.Cpf),
                dto.Password))
            .ReturnsAsync(identityResult);

        var result = await _service.RegisterAsync(dto);

        Assert.True(result.Succeeded);
        _repoMock.VerifyAll();
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenUserNotFound()
    {
        var dto = new LoginDto("nonexistent@example.com", "Pwd@123");
        _repoMock.Setup(r => r.FindByEmailAsync(dto.Email))
                 .ReturnsAsync((Person)null);

        var result = await _service.LoginAsync(dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenPasswordInvalid()
    {
        var user = new Person("Test User", "test@example.com", DateTime.UtcNow.AddYears(-30), "52998224725") { UserName = "test@example.com" };
        var dto = new LoginDto(user.UserName, "WrongPassword");

        _repoMock.Setup(r => r.FindByEmailAsync(dto.Email))
                 .ReturnsAsync(user);
        _repoMock.Setup(r => r.CheckPasswordSignInAsync(user, dto.Password))
                 .ReturnsAsync(SignInResult.Failed);

        var result = await _service.LoginAsync(dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsTokenResult_OnValidCredentials()
    {
        var user = new Person("Test User", "test@example.com", DateTime.UtcNow.AddYears(-30), "52998224725") { UserName = "test@example.com", Id = Guid.NewGuid() };
        var dto = new LoginDto(user.UserName, "Valid@123");

        _repoMock.Setup(r => r.FindByEmailAsync(dto.Email))
                 .ReturnsAsync(user);
        _repoMock.Setup(r => r.CheckPasswordSignInAsync(user, dto.Password))
                 .ReturnsAsync(SignInResult.Success);

        var result = await _service.LoginAsync(dto);

        Assert.NotNull(result);
        Assert.NotEmpty(result!.Jwt);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }
}