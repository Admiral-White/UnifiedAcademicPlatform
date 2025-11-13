using MediatR;
using Microsoft.AspNetCore.Identity;
using UAP.Authentication.Application.Commands;
using UAP.Authentication.Domain.Entities;
using UAP.Authentication.Domain.Enums;
using UAP.Authentication.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.Authentication.Application.CommandHandlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            return Result.Failure<Guid>("User with this email already exists");

        // Create new user
        var user = new User(request.Email, request.FirstName, request.LastName, request.UserType);
        
        // Hash password
        var passwordHash = _passwordHasher.HashPassword(user, request.Password);
        user.SetPassword(passwordHash);

        // Add default role based on user type
        var defaultRole = GetDefaultRole(request.UserType);
        user.AddRole(defaultRole);

        // Save user
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }

    private Guid GetDefaultRole(UserType userType)
    {
        return userType switch
        {
            UserType.Student => Guid.Parse("11111111-1111-1111-1111-111111111111"), // Student role ID
            UserType.Staff => Guid.Parse("22222222-2222-2222-2222-222222222222"), // Staff role ID
            UserType.Administrator => Guid.Parse("33333333-3333-3333-3333-333333333333"), // Admin role ID
            UserType.CourseCoordinator => Guid.Parse("44444444-4444-4444-4444-444444444444"), // Coordinator role ID
            _ => Guid.Parse("11111111-1111-1111-1111-111111111111")
        };
    }
}