using Moq;
using PeopleRegistration.Domain.Entities;
using PeopleRegistration.Application.DTOs;
using PeopleRegistration.Domain.Interfaces;
using PeopleRegistration.Application.Services;
using PeopleRegistration.Application.Factories;

namespace PeopleRegistration.Tests;

public class PersonServiceTests
{
    private readonly Mock<IPersonRepository> _repoMock;
    private readonly Mock<IPersonFactory> _factoryMock;
    private readonly PersonService _service;

    public PersonServiceTests()
    {
        _repoMock    = new(MockBehavior.Strict);
        _factoryMock = new(MockBehavior.Strict);
        _service     = new PersonService(_repoMock.Object, _factoryMock.Object);
    }

    [Fact]
    public async Task ListAsync_ReturnsPagedResponse_WithSeedData()
    {
        var p1 = new Person(
            name: "Alice Silva",
            email: "alice@example.com",
            birthDate: new DateTime(1985, 3, 10),
            cpf: "52998224725")
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserName = "alice",
        };
        p1.SetAddress("Rua A, 100");

        var p2 = new Person(
            name: "Bruno Souza",
            email: "bruno@example.com",
            birthDate: new DateTime(1990, 7, 22),
            cpf: "11144477735")
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            UserName = "bruno",
        };

        var all = new List<Person> { p1, p2 };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(all);
        _factoryMock.Setup(f => f.ToDto(p1)).Returns(new PersonDto(
            p1.Id, p1.Name, p1.Gender, p1.Email,
            p1.BirthDate, p1.BirthPlace, p1.Nationality,
            p1.Cpf, p1.CreatedAt, p1.UpdatedAt, p1.Address));
        _factoryMock.Setup(f => f.ToDto(p2)).Returns(new PersonDto(
            p2.Id, p2.Name, p2.Gender, p2.Email,
            p2.BirthDate, p2.BirthPlace, p2.Nationality,
            p2.Cpf, p2.CreatedAt, p2.UpdatedAt, p2.Address));

        int pageNumber = 1;
        int pageSize = 10;

        // Act
        var result = await _service.ListAsync(pageNumber, pageSize);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(2, result.TotalRecords);
        Assert.Equal(1, result.TotalPages);
        Assert.Collection(result.Data,
            dto =>
            {
                Assert.Equal(p1.Id, dto.Id);
                Assert.Equal("Alice Silva", dto.Name);
                Assert.Equal("alice@example.com", dto.Email);
                Assert.Equal("52998224725", dto.Cpf);
                Assert.Equal("Rua A, 100", dto.Address);
            },
            dto =>
            {
                Assert.Equal(p2.Id, dto.Id);
                Assert.Equal("Bruno Souza", dto.Name);
                Assert.Equal("bruno@example.com", dto.Email);
                Assert.Equal("11144477735", dto.Cpf);
                Assert.Null(dto.Address);
            });

        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        _factoryMock.Verify(f => f.ToDto(p1), Times.Once);
        _factoryMock.Verify(f => f.ToDto(p2), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenCpfExists()
    {
        var dto = new CreatePersonDto(
            "Test", null, "test@example.com",
            DateTime.UtcNow.AddYears(-20), null, null,
            "52998224725");

        _repoMock.Setup(r => r.GetByCpfAsync(dto.Cpf))
                 .ReturnsAsync(new Person(dto.Name, dto.Email, dto.BirthDate, dto.Cpf));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));

        _repoMock.Verify(r => r.GetByCpfAsync(dto.Cpf), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsDto_OnSuccess()
    {
        var dto = new CreatePersonDto("Name", null, null,
            DateTime.UtcNow.AddYears(-30), null, null, "52998224725");
        var person = new Person(dto.Name, dto.Email, dto.BirthDate, dto.Cpf);
        var personDto = new PersonDto(person.Id, person.Name, null, null,
            person.BirthDate, null, null, person.Cpf,
            person.CreatedAt, person.UpdatedAt, null);

        _repoMock.Setup(r => r.GetByCpfAsync(dto.Cpf)).ReturnsAsync((Person)null);
        _factoryMock.Setup(f => f.Create(dto)).Returns(person);
        _repoMock.Setup(r => r.AddAsync(person)).ReturnsAsync(person);
        _factoryMock.Setup(f => f.ToDto(person)).Returns(personDto);

        var result = await _service.CreateAsync(dto);

        Assert.Equal(personDto, result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenNotFound()
    {
        var id = Guid.NewGuid();
        var dto = new CreatePersonDto("Name", null, null,
            DateTime.UtcNow.AddYears(-30), null, null, "52998224725");

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Person)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateAsync(id, dto)
        );
    }

    [Fact]
    public async Task UpdateV2Async_ShouldThrow_WhenNotFound()
    {
        var id = Guid.NewGuid();
        var dto = new CreatePersonV2Dto("NameV2", null, null,
            DateTime.UtcNow.AddYears(-30), null, null, "52998224725", "Some Address");

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Person)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateV2Async(id, dto)
        );
    }


    [Fact]
    public async Task UpdateAsync_ReturnsDto_OnSuccess()
    {
        var dto = new CreatePersonDto("Name", null, null,
            DateTime.UtcNow.AddYears(-30), null, null, "52998224725");
        var id = Guid.NewGuid();
        var person = new Person(dto.Name, dto.Email, dto.BirthDate, dto.Cpf);
        var personDto = new PersonDto(person.Id, person.Name, null, null,
            person.BirthDate, null, null, person.Cpf,
            person.CreatedAt, person.UpdatedAt, null);

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(person);
        _factoryMock.Setup(f => f.Update(person, dto));
        _repoMock.Setup(r => r.UpdateAsync(person)).Returns(Task.CompletedTask);
        _factoryMock.Setup(f => f.ToDto(person)).Returns(personDto);

        var result = await _service.UpdateAsync(id, dto);
        Assert.Equal(personDto, result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Person)null);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(id));
    }

    [Fact]
    public async Task DeleteAsync_CallsRepository_WhenFound()
    {
        var id = Guid.NewGuid();
        var person = new Person("Name", null, DateTime.UtcNow.AddYears(-30), "52998224725");
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(person);
        _repoMock.Setup(r => r.DeleteAsync(person)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(id);
        _repoMock.Verify(r => r.DeleteAsync(person), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Person)null);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var person = new Person("Name", null, DateTime.UtcNow.AddYears(-40), "52998224725");
        var personDto = new PersonDto(person.Id, person.Name, null, null,
            person.BirthDate, null, null, person.Cpf,
            person.CreatedAt, person.UpdatedAt, null);
        _repoMock.Setup(r => r.GetByIdAsync(person.Id)).ReturnsAsync(person);
        _factoryMock.Setup(f => f.ToDto(person)).Returns(personDto);

        var result = await _service.GetByIdAsync(person.Id);
        Assert.Equal(personDto, result);
    }

    [Fact]
    public async Task CreateV2Async_ShouldThrow_WhenCpfExists()
    {
        var dto = new CreatePersonV2Dto("NameV2", null, null,
            DateTime.UtcNow.AddYears(-30), null, null, "52998224725", "Addr V2");
        _repoMock.Setup(r => r.GetByCpfAsync(dto.Cpf))
                 .ReturnsAsync(new Person(dto.Name, dto.Email, dto.BirthDate, dto.Cpf));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateV2Async(dto));
    }

    [Fact]
    public async Task CreateV2Async_ReturnsDto_OnSuccess()
    {
        var dto = new CreatePersonV2Dto("NameV2", null, null,
            DateTime.UtcNow.AddYears(-30), null, null, "52998224725", "Addr V2");
        var person = new Person(dto.Name, dto.Email, dto.BirthDate, dto.Cpf);
        person.SetAddress(dto.Address);
        var personDto = new PersonDto(person.Id, person.Name, null, null,
            person.BirthDate, null, null, person.Cpf,
            person.CreatedAt, person.UpdatedAt, person.Address);

        _repoMock.Setup(r => r.GetByCpfAsync(dto.Cpf)).ReturnsAsync((Person)null);
        _factoryMock.Setup(f => f.CreateV2(dto)).Returns(person);
        _repoMock.Setup(r => r.AddAsync(person)).ReturnsAsync(person);
        _factoryMock.Setup(f => f.ToDto(person)).Returns(personDto);

        var result = await _service.CreateV2Async(dto);
        Assert.Equal(personDto, result);
    }

    [Fact]
    public async Task UpdateV2Async_ReturnsDto_OnSuccess()
    {
        var dto = new CreatePersonV2Dto("NameV2", null, null,
            DateTime.UtcNow.AddYears(-30), null, null, "52998224725", "Addr V2");
        var id = Guid.NewGuid();
        var person = new Person(dto.Name, dto.Email, dto.BirthDate, dto.Cpf);
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(person);
        _factoryMock.Setup(f => f.UpdateV2(person, dto));
        _repoMock.Setup(r => r.UpdateAsync(person)).Returns(Task.CompletedTask);
        var personDto = new PersonDto(person.Id, person.Name, null, null,
            person.BirthDate, null, null, person.Cpf,
            person.CreatedAt, person.UpdatedAt, dto.Address);
        _factoryMock.Setup(f => f.ToDto(person)).Returns(personDto);

        var result = await _service.UpdateV2Async(id, dto);
        Assert.Equal(personDto, result);
    }
}