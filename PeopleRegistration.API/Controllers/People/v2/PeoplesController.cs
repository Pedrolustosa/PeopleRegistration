using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PeopleRegistration.Application.DTOs;
using PeopleRegistration.Application.Interfaces;

namespace PeopleRegistration.API.Controllers.People.v2;

[ApiController]
[ApiVersion("2.0")]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class PeoplesController(IPersonService service) : ControllerBase
{
    private readonly IPersonService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.ListAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(CreatePersonV2Dto dto)
    {
        var created = await _service.CreateV2Async(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id, version = "2.0" }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CreatePersonV2Dto dto)
        => Ok(await _service.UpdateV2Async(id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
