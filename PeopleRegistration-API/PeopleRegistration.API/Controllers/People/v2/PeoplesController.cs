using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PeopleRegistration.Application.DTOs;
using PeopleRegistration.Application.Common;
using PeopleRegistration.Application.Interfaces;

namespace PeopleRegistration.API.Controllers.People.v2;

[ApiController]
[ApiVersion("2.0")]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class PeoplesController(IPersonService service) : ControllerBase
{
    private readonly IPersonService _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonV2Dto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage);
            return BadRequest(ApiResponse<EmptyClass>.Fail(errors, ApiMessages.ValidationFailed));
        }

        try
        {
            var created = await _service.CreateV2Async(dto);
            return CreatedAtAction(
                nameof(Create),
                new { id = created.Id, version = "2.0" },
                ApiResponse<PersonDto>.Ok(created, ApiMessages.PersonCreated)
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<EmptyClass>.Fail(
                [ex.Message], ApiMessages.RegistrationError));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName ?? string.Empty, ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<EmptyClass>.Fail([ex.Message], ApiMessages.UnexpectedError));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreatePersonV2Dto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updated = await _service.UpdateV2Async(id, dto);
            return Ok(ApiResponse<PersonDto>.Ok(updated, ApiMessages.PersonUpdated));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<EmptyClass>.Fail(
                [ApiMessages.PersonNotFound]));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName ?? string.Empty, ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<EmptyClass>.Fail(
                [ex.Message], ApiMessages.RegistrationError));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<EmptyClass>.Fail([ex.Message], ApiMessages.UnexpectedError));
        }
    }
}
