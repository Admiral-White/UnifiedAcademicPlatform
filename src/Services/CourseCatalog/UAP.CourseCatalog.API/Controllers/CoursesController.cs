using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UAP.CourseCatalog.Application.Commands;
using UAP.CourseCatalog.Application.Queries;
using UAP.Shared.Contracts.Common;

namespace UAP.CourseCatalog.API.Controllers;

[ApiController]
[Route(ApiRoutes.Courses.GetAll)]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(IMediator mediator, ILogger<CoursesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourses(
        [FromQuery] string search = null,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] string semester = null,
        [FromQuery] int? academicYear = null,
        [FromQuery] bool? isBorrowable = null,
        [FromQuery] bool? hasAvailableCapacity = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation(
            "Getting courses - Search: {Search}, Department: {DepartmentId}, Semester: {Semester}",
            search, departmentId, semester);

        var query = new GetCoursesQuery(
            search, departmentId, semester, academicYear, 
            isBorrowable, hasAvailableCapacity, page, pageSize);
        
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(new { 
                data = result.Value,
                totalCount = result.Value.Count,
                page = page,
                pageSize = pageSize
            });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        _logger.LogInformation("Getting course: {CourseId}", id);
        
        var query = new GetCourseQuery(id);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return NotFound(new { Error = result.Error });
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,CourseCoordinator")]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseCommand command)
    {
        _logger.LogInformation("Creating course: {CourseCode}", command.CourseCode);
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetCourse), new { id = result.Value }, new { CourseId = result.Value });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrator,CourseCoordinator")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseCommand command)
    {
        if (id != command.CourseId)
            return BadRequest(new { Error = "Course ID mismatch" });

        _logger.LogInformation("Updating course: {CourseId}", id);
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { Message = "Course updated successfully" });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrator,CourseCoordinator")]
    public async Task<IActionResult> DeactivateCourse(Guid id, [FromBody] string reason = null)
    {
        _logger.LogInformation("Deactivating course: {CourseId}, Reason: {Reason}", id, reason);
        
        var command = new DeactivateCourseCommand(id, reason);
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { Message = "Course deactivated successfully" });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpPatch("{id:guid}/capacity")]
    [Authorize(Roles = "Administrator,CourseCoordinator")]
    public async Task<IActionResult> UpdateCourseCapacity(Guid id, [FromBody] UpdateCourseCapacityCommand command)
    {
        if (id != command.CourseId)
            return BadRequest(new { Error = "Course ID mismatch" });

        _logger.LogInformation("Updating course capacity: {CourseId} to {NewCapacity}", id, command.NewCapacity);
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { Message = "Course capacity updated successfully" });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("coordinator/{coordinatorId:guid}")]
    [Authorize(Roles = "Administrator,CourseCoordinator")]
    public async Task<IActionResult> GetCoursesByCoordinator(Guid coordinatorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting courses by coordinator: {CoordinatorId}", coordinatorId);
        
        var query = new GetCoursesQuery(null, null, null, null, null, null, page, pageSize);
        // Note: This would need a specific query for coordinator courses
        // For now, using the general query as placeholder
        
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(new { 
                data = result.Value,
                page = page,
                pageSize = pageSize
            });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("borrowable")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBorrowableCourses([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting borrowable courses");
        
        var query = new GetCoursesQuery(null, null, null, null, true, null, page, pageSize);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(new { 
                data = result.Value,
                page = page,
                pageSize = pageSize
            });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("available")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoursesWithAvailableCapacity([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting courses with available capacity");
        
        var query = new GetCoursesQuery(null, null, null, null, null, true, page, pageSize);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(new { 
                data = result.Value,
                page = page,
                pageSize = pageSize
            });
        }
        
        return BadRequest(new { Error = result.Error });
    }
}