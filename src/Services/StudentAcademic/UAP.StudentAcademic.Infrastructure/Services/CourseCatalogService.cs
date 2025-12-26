using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using UAP.SharedKernel.Common;

namespace UAP.StudentAcademic.Infrastructure.Services;

public class CourseCatalogService : ICourseCatalogService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CourseCatalogService> _logger;

    public CourseCatalogService(HttpClient httpClient, ILogger<CourseCatalogService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<CourseDto>> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/courses/{courseId}", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
                return Result.Failure<CourseDto>($"Failed to get course: {response.StatusCode}");

            var course = await response.Content.ReadFromJsonAsync<CourseDto>(cancellationToken);
            return Result.Success(course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course {CourseId}", courseId);
            return Result.Failure<CourseDto>($"Error getting course: {ex.Message}");
        }
    }

    public async Task<Result<List<CourseDto>>> GetCoursesByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/courses/department/{departmentId}", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
                return Result.Failure<List<CourseDto>>($"Failed to get courses: {response.StatusCode}");

            var courses = await response.Content.ReadFromJsonAsync<List<CourseDto>>(cancellationToken);
            return Result.Success(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting courses for department {DepartmentId}", departmentId);
            return Result.Failure<List<CourseDto>>($"Error getting courses: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ValidateCoursePrerequisitesAsync(Guid courseId, List<Guid> passedCourseIds, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/courses/{courseId}/validate-prerequisites", passedCourseIds, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
                return Result.Failure<bool>($"Failed to validate prerequisites: {response.StatusCode}");

            var isValid = await response.Content.ReadFromJsonAsync<bool>(cancellationToken);
            return Result.Success(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating prerequisites for course {CourseId}", courseId);
            return Result.Failure<bool>($"Error validating prerequisites: {ex.Message}");
        }
    }
}
