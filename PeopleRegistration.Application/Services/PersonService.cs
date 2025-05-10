using PeopleRegistration.Application.DTOs;
using PeopleRegistration.Domain.Interfaces;
using PeopleRegistration.Application.Factories;
using PeopleRegistration.Application.Interfaces;

namespace PeopleRegistration.Application.Services;

public class PersonService(IPersonRepository repo, IPersonFactory personFactory) : IPersonService
{
    private readonly IPersonRepository _repo = repo;
    private readonly IPersonFactory _factory = personFactory;

    public async Task<PersonDto> CreateAsync(CreatePersonDto dto)
    {
        if (await _repo.GetByCpfAsync(dto.Cpf) is not null)
            throw new InvalidOperationException("CPF already exists.");

        var person = _factory.Create(dto);
        var added = await _repo.AddAsync(person);
        return _factory.ToDto(added);
    }

    public async Task<PersonDto> UpdateAsync(Guid id, CreatePersonDto dto)
    {
        var person = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Person not found.");

        _factory.Update(person, dto);

        await _repo.UpdateAsync(person);
        return _factory.ToDto(person);
    }

    public async Task<PersonDto> CreateV2Async(CreatePersonV2Dto dto)
    {
        if (await _repo.GetByCpfAsync(dto.Cpf) is not null)
            throw new InvalidOperationException("CPF already exists.");

        var person = _factory.CreateV2(dto);
        var added = await _repo.AddAsync(person);
        return _factory.ToDto(added);
    }

    public async Task<PersonDto> UpdateV2Async(Guid id, CreatePersonV2Dto dto)
    {
        var person = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Person not found.");

        _factory.UpdateV2(person, dto);

        await _repo.UpdateAsync(person);
        return _factory.ToDto(person);
    }

    public async Task DeleteAsync(Guid id)
    {
        var person = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Person not found.");

        await _repo.DeleteAsync(person);
    }

    public async Task<PersonDto> GetByIdAsync(Guid id)
    {
        var person = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Person not found.");

        return _factory.ToDto(person);
    }

    public async Task<IEnumerable<PersonDto>> ListAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(p => _factory.ToDto(p));
    }
}
