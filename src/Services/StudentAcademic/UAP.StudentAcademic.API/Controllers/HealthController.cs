using Microsoft.AspNetCore.Mvc;

namespace UAP.StudentAcademic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "StudentAcademic Service"
        });
    }
}