using MediatR;
using UAP.Authentication.Domain.Enums;
using UAP.SharedKernel.Common;

namespace UAP.Authentication.Application.Commands;

public class RegisterUserCommand : IRequest<Result<Guid>>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserType UserType { get; set; }
    
    public RegisterUserCommand(string email, string password, string firstName, string lastName, UserType userType)
    {
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        UserType = userType;
    }
}