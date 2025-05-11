using PeopleRegistration.Domain.Entities;
using PeopleRegistration.Application.DTOs;
using PeopleRegistration.Application.Factories;

namespace PeopleRegistration.Infrastructure.Factories;

public class PersonFactory : IPersonFactory
{
    public Person Create(CreatePersonDto dto)
    {
        var person = new Person(
            dto.Name,
            dto.Email,
            dto.BirthDate,
            dto.Cpf
        );

        person.UpdateProfile(
            dto.Name,
            dto.Gender,
            dto.BirthDate,
            dto.BirthPlace,
            dto.Nationality,
            null
        );

        return person;
    }

    public Person CreateV2(CreatePersonV2Dto dto)
    {
        var person = new Person(
            dto.Name,
            dto.Email,
            dto.BirthDate,
            dto.Cpf
        );

        person.UpdateProfile(
            dto.Name,
            dto.Gender,
            dto.BirthDate,
            dto.BirthPlace,
            dto.Nationality,
            dto.Address
        );

        return person;
    }

    public void Update(Person person, CreatePersonDto dto)
    {
        if (person == null) throw new ArgumentNullException(nameof(person));

        person.UpdateProfile(
            dto.Name,
            dto.Gender,
            dto.BirthDate,
            dto.BirthPlace,
            dto.Nationality,
            person.Address
        );
    }

    public void UpdateV2(Person person, CreatePersonV2Dto dto)
    {
        if (person == null) throw new ArgumentNullException(nameof(person));

        person.UpdateProfile(
            dto.Name,
            dto.Gender,
            dto.BirthDate,
            dto.BirthPlace,
            dto.Nationality,
            dto.Address
        );
    }

    public PersonDto ToDto(Person person)
    {
        if (person == null) throw new ArgumentNullException(nameof(person));

        return new PersonDto(
            person.Id,
            person.Name,
            person.Gender,
            person.Email,
            person.BirthDate,
            person.BirthPlace,
            person.Nationality,
            person.Cpf,
            person.CreatedAt,
            person.UpdatedAt,
            person.Address
        );
    }
}