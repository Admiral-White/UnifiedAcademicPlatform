using UAP.SharedKernel.Common;

namespace UAP.StudentAcademic.Infrastructure.Services;

public interface IAuthenticationService
{
    Task<Result<UserInfoDto>> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<Result<UserInfoDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class UserInfoDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserType { get; set; }
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
}