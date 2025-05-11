using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PeopleRegistration.Application.DTOs;
using PeopleRegistration.Application.Common;
using PeopleRegistration.Application.Interfaces;

namespace PeopleRegistration.API.Controllers.People.v1;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class PeoplesController(IPersonService service) : ControllerBase
{
    private readonly IPersonService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        try
        {
            var paged = await _service.ListAsync(pageNumber, pageSize);
            return Ok(paged);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<EmptyClass>.Fail(
                    new[] { ex.Message },
                    ApiMessages.UnexpectedError));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var person = await _service.GetByIdAsync(id);
            return Ok(ApiResponse<PersonDto>.Ok(person));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<EmptyClass>.Fail(
                new[] { ApiMessages.PersonNotFound }));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<EmptyClass>.Fail(new[] { ex.Message }, ApiMessages.UnexpectedError));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(ApiResponse<EmptyClass>.Fail(errors, ApiMessages.ValidationFailed));
        }

        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id, version = "1.0" },
                ApiResponse<PersonDto>.Ok(created, ApiMessages.PersonCreated)
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<EmptyClass>.Fail(
                new[] { ex.Message }, ApiMessages.RegistrationError));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName ?? string.Empty, ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<EmptyClass>.Fail(new[] { ex.Message }, ApiMessages.UnexpectedError));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreatePersonDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(ApiResponse<PersonDto>.Ok(updated, ApiMessages.PersonUpdated));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<EmptyClass>.Fail(
                new[] { ApiMessages.PersonNotFound }));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName ?? string.Empty, ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<EmptyClass>.Fail(
                new[] { ex.Message }, ApiMessages.RegistrationError));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<EmptyClass>.Fail(new[] { ex.Message }, ApiMessages.UnexpectedError));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse<EmptyClass>.Ok(null!, ApiMessages.PersonDeleted));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<EmptyClass>.Fail(
                new[] { ApiMessages.PersonNotFound }));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<EmptyClass>.Fail(new[] { ex.Message }, ApiMessages.UnexpectedError));
        }
    }
}
