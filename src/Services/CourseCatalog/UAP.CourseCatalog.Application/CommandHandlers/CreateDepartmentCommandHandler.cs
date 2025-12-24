using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Application.Commands;
using UAP.CourseCatalog.Domain.Entities;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.CommandHandlers;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<Guid>>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDepartmentCommandHandler> _logger;

    public CreateDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateDepartmentCommandHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new department: {Code}", request.Code);

            // Check if department code already exists
            var existingDepartment = await _departmentRepository.GetByCodeAsync(request.Code, cancellationToken);
            if (existingDepartment != null)
                return Result.Failure<Guid>($"Department with code {request.Code} already exists");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result.Failure<Guid>("Department name is required");

            // Create department
            var department = new Department(
                request.Code,
                request.Name,
                request.Description,
                request.HeadOfDepartmentId
            );

            // Save department
            await _departmentRepository.AddAsync(department, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Department created successfully: {DepartmentId} - {Code}", department.Id, department.Code);
            
            return Result.Success(department.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department: {Code}", request.Code);
            return Result.Failure<Guid>("An error occurred while creating the department");
        }
    }
}