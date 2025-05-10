using PeopleRegistration.Domain.Enum;

namespace PeopleRegistration.Application.DTOs;

public record PersonDto(
        Guid Id,
        string Name,
        Gender? Gender,
        string? Email,
        DateTime BirthDate,
        string? BirthPlace,
        string? Nationality,
        string Cpf,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        string? Address
    );
