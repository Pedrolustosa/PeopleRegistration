using PeopleRegistration.Domain.Enum;

namespace PeopleRegistration.Application.DTOs;

public record CreatePersonV2Dto(
        string Name,
        Gender? Gender,
        string? Email,
        DateTime BirthDate,
        string? BirthPlace,
        string? Nationality,
        string Cpf,
        string Address
    );
