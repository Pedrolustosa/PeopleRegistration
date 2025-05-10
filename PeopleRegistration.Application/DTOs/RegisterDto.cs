namespace PeopleRegistration.Application.DTOs;

public record RegisterDto(
        string Name,
        string Username,
        string? Email,
        DateTime BirthDate,
        string Cpf,
        string Password
    );
