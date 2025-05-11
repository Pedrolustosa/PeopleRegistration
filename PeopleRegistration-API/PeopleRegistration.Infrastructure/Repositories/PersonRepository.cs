using Microsoft.EntityFrameworkCore;
using PeopleRegistration.Domain.Entities;
using PeopleRegistration.Domain.Interfaces;
using PeopleRegistration.Infrastructure.Data;

namespace PeopleRegistration.Infrastructure.Repositories;

public class PersonRepository(ApplicationDbContext ctx) : IPersonRepository
{
    private readonly ApplicationDbContext _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));

    public async Task<Person> AddAsync(Person entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        try
        {
            var entry = await _ctx.People.AddAsync(entity);
            await _ctx.SaveChangesAsync();
            return entry.Entity;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error adding person: " + ex.Message, ex);
        }
    }

    public async Task DeleteAsync(Person entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        try
        {
            _ctx.People.Remove(entity);
            await _ctx.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error deleting person: " + ex.Message, ex);
        }
    }

    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        try
        {
            return await _ctx.People.OrderByDescending(x => x.CreatedAt).AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving people: " + ex.Message, ex);
        }
    }

    public async Task<Person?> GetByCpfAsync(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF is required.", nameof(cpf));

        try
        {
            return await _ctx.People.FirstOrDefaultAsync(p => p.Cpf == cpf);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error finding person by CPF: " + ex.Message, ex);
        }
    }

    public async Task<Person?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID is required.", nameof(id));

        try
        {
            return await _ctx.People.FindAsync(id);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error finding person by ID: " + ex.Message, ex);
        }
    }

    public async Task UpdateAsync(Person entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        try
        {
            _ctx.People.Update(entity);
            await _ctx.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error updating person: " + ex.Message, ex);
        }
    }
}
