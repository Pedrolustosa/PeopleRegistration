using PeopleRegistration.Domain.Entities;

namespace PeopleRegistration.Domain.Interfaces;

public interface IPersonRepository
{
    Task<Person> AddAsync(Person person);
    Task<Person?> GetByIdAsync(Guid id);
    Task<Person?> GetByCpfAsync(string cpf);
    Task<IEnumerable<Person>> GetAllAsync();
    Task UpdateAsync(Person person);
    Task DeleteAsync(Person person);
}
