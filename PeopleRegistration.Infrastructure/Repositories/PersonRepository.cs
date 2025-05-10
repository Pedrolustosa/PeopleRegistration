using Microsoft.EntityFrameworkCore;
using PeopleRegistration.Domain.Entities;
using PeopleRegistration.Domain.Interfaces;
using PeopleRegistration.Infrastructure.Data;

namespace PeopleRegistration.Infrastructure.Repositories;

public class PersonRepository(ApplicationDbContext ctx) : IPersonRepository
{
    private readonly ApplicationDbContext _ctx = ctx;

    public async Task<Person> AddAsync(Person entity)
    {
        var e = (await _ctx.People.AddAsync(entity)).Entity;
        await _ctx.SaveChangesAsync();
        return e;
    }
    public async Task DeleteAsync(Person entity)
    {
        _ctx.People.Remove(entity);
        await _ctx.SaveChangesAsync();
    }
    public async Task<IEnumerable<Person>> GetAllAsync()
        => await _ctx.People.ToListAsync();

    public async Task<Person?> GetByCpfAsync(string cpf)
        => await _ctx.People.FirstOrDefaultAsync(p => p.Cpf == cpf);

    public async Task<Person?> GetByIdAsync(Guid id)
        => await _ctx.People.FindAsync(id);

    public async Task UpdateAsync(Person entity)
    {
        _ctx.People.Update(entity);
        await _ctx.SaveChangesAsync();
    }
}
