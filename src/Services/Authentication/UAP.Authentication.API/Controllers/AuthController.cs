using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UAP.Authentication.Application.Commands;
using UAP.Shared.Contracts.Common;

namespace UAP.Authentication.API.Controllers;

[ApiController]
[Route(ApiRoutes.Authentication.Login)]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        _logger.LogInformation("Registering new user with email: {Email}", command.Email);
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("User registered successfully with ID: {UserId}", result.Value);
            return Ok(new { UserId = result.Value, Message = "User registered successfully" });
        }
        
        _logger.LogWarning("User registration failed: {Error}", result.Error);
        return BadRequest(new { Error = result.Error });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        _logger.LogInformation("Login attempt for email: {Email}", command.Email);
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("User logged in successfully: {Email}", command.Email);
            return Ok(result.Value);
        }
        
        _logger.LogWarning("Login failed for email: {Email}, Error: {Error}", command.Email, result.Error);
        return Unauthorized(new { Error = result.Error });
    }

    [HttpPost("validate")]
    [Authorize]
    public IActionResult Validate()
    {
        // If we reach here, the token is valid
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        
        _logger.LogInformation("Token validation successful for user: {Email}", email);
        
        return Ok(new 
        { 
            UserId = userId,
            Email = email,
            IsAuthenticated = true,
            Message = "Token is valid"
        });
    }

    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var firstName = User.FindFirst("given_name")?.Value;
        var lastName = User.FindFirst("family_name")?.Value;
        var userType = User.FindFirst("user_type")?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new
        {
            UserId = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            UserType = userType,
            Roles = roles
        });
    }
    
    
    [HttpPost("deactivate/{userId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeactivateUser(Guid userId, [FromBody] string reason = null)
    {
        _logger.LogInformation("Deactivating user: {UserId}, Reason: {Reason}", userId, reason);
    
        var command = new DeactivateUserCommand(userId, reason);
        var result = await _mediator.Send(command);
    
        if (result.IsSuccess)
        {
            _logger.LogInformation("User deactivated successfully: {UserId}", userId);
            return Ok(new { Message = "User deactivated successfully" });
        }
    
        _logger.LogWarning("User deactivation failed: {UserId}, Error: {Error}", userId, result.Error);
        return BadRequest(new { Error = result.Error });
    }
}