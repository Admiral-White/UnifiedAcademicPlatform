using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UAP.Authentication.API.Controllers
{

    /// <summary>
    /// Health check endpoint for service monitoring and load balancers
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            var report = await _healthCheckService.CheckHealthAsync();

            var status = report.Status == HealthStatus.Healthy ?
                "Healthy" : "Unhealthy";

            return Ok(new
            {
                status = status,
                timestamp = DateTime.UtcNow,
                service = "Authentication Service",
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description
                })
            });
        }

    }
}
