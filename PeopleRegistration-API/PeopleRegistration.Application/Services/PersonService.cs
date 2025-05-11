using PeopleRegistration.Application.DTOs;
using PeopleRegistration.Domain.Interfaces;
using PeopleRegistration.Application.Factories;
using PeopleRegistration.Application.Interfaces;

namespace PeopleRegistration.Application.Services;

public class PersonService(IPersonRepository repo, IPersonFactory personFactory) : IPersonService
{
    private readonly IPersonRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IPersonFactory _factory = personFactory ?? throw new ArgumentNullException(nameof(personFactory));

    public async Task<PersonDto> CreateAsync(CreatePersonDto dto)
    {
        ValidateDto(dto);

        if (await _repo.GetByCpfAsync(dto.Cpf) is not null)
            throw new InvalidOperationException("CPF already exists.");

        try
        {
            var person = _factory.Create(dto);
            var added = await _repo.AddAsync(person);
            return _factory.ToDto(added);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error creating person: " + ex.Message, ex);
        }
    }

    public async Task<PersonDto> UpdateAsync(Guid id, CreatePersonDto dto)
    {
        ValidateDto(dto);

        var person = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Person not found.");

        try
        {
            _factory.Update(person, dto);
            await _repo.UpdateAsync(person);
            return _factory.ToDto(person);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error updating person: " + ex.Message, ex);
        }
    }

    public async Task<PersonDto> CreateV2Async(CreatePersonV2Dto dto)
    {
        ValidateDto(dto);

        if (await _repo.GetByCpfAsync(dto.Cpf) is not null)
            throw new InvalidOperationException("CPF already exists.");

        try
        {
            var person = _factory.CreateV2(dto);
            var added = await _repo.AddAsync(person);
            return _factory.ToDto(added);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error creating person (v2): " + ex.Message, ex);
        }
    }

    public async Task<PersonDto> UpdateV2Async(Guid id, CreatePersonV2Dto dto)
    {
        ValidateDto(dto);

        var person = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Person not found.");

        try
        {
            _factory.UpdateV2(person, dto);
            await _repo.UpdateAsync(person);
            return _factory.ToDto(person);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error updating person (v2): " + ex.Message, ex);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var person = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Person not found.");

        try
        {
            await _repo.DeleteAsync(person);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error deleting person: " + ex.Message, ex);
        }
    }

    public async Task<PersonDto> GetByIdAsync(Guid id)
    {
        var person = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Person not found.");

        return _factory.ToDto(person);
    }

    public async Task<PagedResponse<PersonDto>> ListAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize   = 10;

        var all = await _repo.GetAllAsync();
        var totalCount = all.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = all
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => _factory.ToDto(p))
            .ToList();

        return new PagedResponse<PersonDto>
        {
            Success      = true,
            Data         = items,
            PageNumber   = pageNumber,
            PageSize     = pageSize,
            TotalPages   = totalPages,
            TotalRecords = totalCount
        };
    }

    private static void ValidateDto(CreatePersonDto dto)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.", nameof(dto.Name));
        if (dto.BirthDate >= DateTime.UtcNow)
            throw new ArgumentException("BirthDate must be in the past.", nameof(dto.BirthDate));
        if (string.IsNullOrWhiteSpace(dto.Cpf))
            throw new ArgumentException("CPF is required.", nameof(dto.Cpf));
    }

    private static void ValidateDto(CreatePersonV2Dto dto)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.", nameof(dto.Name));
        if (dto.BirthDate >= DateTime.UtcNow)
            throw new ArgumentException("BirthDate must be in the past.", nameof(dto.BirthDate));
        if (string.IsNullOrWhiteSpace(dto.Cpf))
            throw new ArgumentException("CPF is required.", nameof(dto.Cpf));
        if (string.IsNullOrWhiteSpace(dto.Address))
            throw new ArgumentException("Address is required.", nameof(dto.Address));
    }
}
