using Microsoft.AspNetCore.Identity;
using PeopleRegistration.Domain.Enum;

namespace PeopleRegistration.Domain.Entities;

public class Person : IdentityUser<Guid>
{
    public string Name { get; private set; }
    public Gender? Gender { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string? BirthPlace { get; private set; }
    public string? Nationality { get; private set; }
    public string Cpf { get; private set; }
    public string? Address { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private Person() { }

    public Person(string name, string? email, DateTime birthDate, string cpf)
        : base()
    {
        SetName(name);
        SetEmail(email);
        SetBirthDate(birthDate);
        SetCpf(cpf);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(
        string name,
        Gender? gender,
        DateTime birthDate,
        string? birthPlace,
        string? nationality,
        string? address)
    {
        SetName(name);
        Gender = gender;
        SetBirthDate(birthDate);
        BirthPlace = birthPlace;
        Nationality = nationality;
        if (!string.IsNullOrWhiteSpace(address))
            SetAddress(address);
        Touch();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
            throw new ArgumentException("Name must be non-empty and up to 200 chars.", nameof(name));
        Name = name.Trim();
    }

    private void SetEmail(string? email)
    {
        if (email != null)
        {
            var addr = new System.Net.Mail.MailAddress(email.Trim());
            Email = addr.Address;
            UserName = addr.Address;
        }
    }

    private void SetBirthDate(DateTime date)
    {
        if (date >= DateTime.UtcNow)
            throw new ArgumentException("BirthDate must be in the past.", nameof(date));
        BirthDate = date.Date;
    }

    private void SetCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11 || !ulong.TryParse(cpf, out _))
            throw new ArgumentException("CPF must consist of 11 numeric digits.", nameof(cpf));
        Cpf = cpf;
    }

    public void SetAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address) || address.Length > 300)
            throw new ArgumentException("Address must be non-empty and up to 300 chars.", nameof(address));
        Address = address.Trim();
        Touch();
    }

    public void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
