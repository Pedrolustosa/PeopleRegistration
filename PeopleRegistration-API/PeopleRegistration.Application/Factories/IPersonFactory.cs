using PeopleRegistration.Domain.Entities;
using PeopleRegistration.Application.DTOs;

namespace PeopleRegistration.Application.Factories;

public interface IPersonFactory
{
    Person Create(CreatePersonDto dto);
    Person CreateV2(CreatePersonV2Dto dto);
    void Update(Person person, CreatePersonDto dto);
    void UpdateV2(Person person, CreatePersonV2Dto dto);
    PersonDto ToDto(Person person);
}
