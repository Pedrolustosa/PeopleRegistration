using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PeopleRegistration.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PeopleRegistration.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<Person, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Person> People { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Person>(e =>
        {
            e.ToTable("People");
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Cpf).IsUnique();
            e.Property(p => p.Name).IsRequired();
            e.Property(p => p.Cpf)
             .IsRequired()
             .HasMaxLength(11);
            e.Property(p => p.BirthDate).IsRequired();
            e.Property(p => p.Address).IsRequired(false);
        });
    }
}
