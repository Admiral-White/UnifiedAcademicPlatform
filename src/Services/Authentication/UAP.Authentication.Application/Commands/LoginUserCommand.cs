using MediatR;
using UAP.Authentication.Domain.Enums;
using UAP.SharedKernel.Common;

namespace UAP.Authentication.Application.Commands;

public class LoginUserCommand : IRequest<Result<LoginResponse>>
{
    public string Email { get; set; }
    public string Password { get; set; }

    public LoginUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}

public class LoginResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserType UserType { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}