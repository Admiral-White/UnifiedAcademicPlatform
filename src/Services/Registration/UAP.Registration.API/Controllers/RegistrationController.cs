using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UAP.Registration.Application.Commands;
using UAP.Registration.Application.Queries;

namespace UAP.Registration.API.Controllers;

[ApiController]
[Route("api/v1/registrations")]
public class RegistrationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RegistrationController> _logger;

    public RegistrationController(IMediator mediator, ILogger<RegistrationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RegisterCourse([FromBody] RegisterCourseCommand command)
    {
        _logger.LogInformation("Registering course {CourseId} for student {StudentId}", command.CourseId, command.StudentId);
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetRegistration), new { id = result.Value }, new { RegistrationId = result.Value });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetRegistration(Guid id)
    {
        _logger.LogInformation("Getting registration: {RegistrationId}", id);
        
        var query = new GetRegistrationByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return NotFound(new { Error = result.Error });
    }

    [HttpGet("student/{studentId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetStudentRegistrations(Guid studentId)
    {
        _logger.LogInformation("Getting registrations for student: {StudentId}", studentId);
        
        var query = new GetStudentRegistrationsQuery(studentId);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("course/{courseId:guid}")]
    [Authorize(Roles = "Administrator,Staff")]
    public async Task<IActionResult> GetCourseRegistrations(Guid courseId)
    {
        _logger.LogInformation("Getting registrations for course: {CourseId}", courseId);
        
        var query = new GetCourseRegistrationsQuery(courseId);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles = "Administrator,Staff,CourseCoordinator")]
    public async Task<IActionResult> ApproveRegistration(Guid id, [FromBody] ApproveCourseRegistrationCommand command)
    {
        if (id != command.RegistrationId)
            return BadRequest(new { Error = "Registration ID mismatch" });

        _logger.LogInformation("Approving registration: {RegistrationId}", id);
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { Message = "Registration approved successfully" });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = "Administrator,Staff,CourseCoordinator")]
    public async Task<IActionResult> RejectRegistration(Guid id, [FromBody] RejectCourseRegistrationCommand command)
    {
        if (id != command.RegistrationId)
            return BadRequest(new { Error = "Registration ID mismatch" });

        _logger.LogInformation("Rejecting registration: {RegistrationId}", id);
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { Message = "Registration rejected successfully" });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> CancelRegistration(Guid id, [FromBody] CancelCourseRegistrationCommand command)
    {
        if (id != command.RegistrationId)
            return BadRequest(new { Error = "Registration ID mismatch" });

        _logger.LogInformation("Cancelling registration: {RegistrationId}", id);
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { Message = "Registration cancelled successfully" });
        }
        
        return BadRequest(new { Error = result.Error });
    }
}