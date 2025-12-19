using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UAP.StudentAcademic.Application.Commands;
using UAP.StudentAcademic.Application.Queries;

namespace UAP.StudentAcademic.API.Controllers;

[ApiController]
[Route("api/v1/students")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IMediator mediator, ILogger<StudentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        _logger.LogInformation("Getting student: {StudentId}", id);
        
        var query = new GetStudentQuery(id);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return NotFound(new { Error = result.Error });
    }

    [HttpGet("user/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetStudentByUserId(Guid userId)
    {
        _logger.LogInformation("Getting student by user ID: {UserId}", userId);
        
        var query = new GetStudentByUserIdQuery(userId);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return NotFound(new { Error = result.Error });
    }

    [HttpGet("{id:guid}/grades")]
    [Authorize]
    public async Task<IActionResult> GetStudentGrades(Guid id, [FromQuery] string semester = null, [FromQuery] int? academicYear = null)
    {
        _logger.LogInformation("Getting grades for student: {StudentId}", id);
        
        var query = new GetStudentGradesQuery(id, semester, academicYear);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("{id:guid}/transcript")]
    [Authorize]
    public async Task<IActionResult> GetStudentTranscript(Guid id, [FromQuery] bool includeInProgress = false)
    {
        _logger.LogInformation("Getting transcript for student: {StudentId}", id);
        
        var query = new CalculateTranscriptQuery(id, includeInProgress);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpPost("{id:guid}/grades")]
    [Authorize(Roles = "Administrator,Staff,CourseCoordinator")]
    public async Task<IActionResult> SubmitGrade(Guid id, [FromBody] SubmitGradeCommand command)
    {
        if (id != command.StudentId)
            return BadRequest(new { Error = "Student ID mismatch" });

        _logger.LogInformation("Submitting grade for student: {StudentId}", id);
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetStudentGrades), new { id }, new { GradeId = result.Value });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Administrator,Staff")]
    public async Task<IActionResult> UpdateStudentStatus(Guid id, [FromBody] UpdateStudentStatusCommand command)
    {
        if (id != command.StudentId)
            return BadRequest(new { Error = "Student ID mismatch" });

        _logger.LogInformation("Updating status for student: {StudentId} to {Status}", id, command.Status);
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { Message = "Student status updated successfully" });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("{id:guid}/cgpa")]
    [Authorize]
    public async Task<IActionResult> CalculateCGPA(Guid id)
    {
        _logger.LogInformation("Calculating CGPA for student: {StudentId}", id);
        
        var command = new CalculateCGPACommand(id);
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { StudentId = id, CGPA = result.Value });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("department/{departmentId:guid}")]
    [Authorize(Roles = "Administrator,Staff")]
    public async Task<IActionResult> GetStudentsByDepartment(Guid departmentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting students by department: {DepartmentId}", departmentId);
        
        // This would use a specific query handler
        return Ok(new { Message = "Endpoint under implementation", DepartmentId = departmentId });
    }

    [HttpGet("probation")]
    [Authorize(Roles = "Administrator,Staff")]
    public async Task<IActionResult> GetProbationStudents([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting probation students");
        
        // This would use a specific query handler
        return Ok(new { Message = "Endpoint under implementation" });
    }

    [HttpGet("deanlist")]
    [Authorize]
    public async Task<IActionResult> GetDeanListStudents([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting dean's list students");
        
        // This would use a specific query handler
        return Ok(new { Message = "Endpoint under implementation" });
    }
}