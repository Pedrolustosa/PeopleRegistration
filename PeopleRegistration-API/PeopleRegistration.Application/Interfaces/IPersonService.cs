using PeopleRegistration.Application.DTOs;

namespace PeopleRegistration.Application.Interfaces;

public interface IPersonService
{
    Task<PersonDto> CreateAsync(CreatePersonDto dto);
    Task<PersonDto> UpdateAsync(Guid id, CreatePersonDto dto);
    Task<PersonDto> CreateV2Async(CreatePersonV2Dto dto);
    Task<PersonDto> UpdateV2Async(Guid id, CreatePersonV2Dto dto);
    Task DeleteAsync(Guid id);
    Task<PersonDto> GetByIdAsync(Guid id);
    Task<PagedResponse<PersonDto>> ListAsync(int pageNumber, int pageSize);

}
