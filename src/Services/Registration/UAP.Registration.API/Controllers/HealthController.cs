using Microsoft.AspNetCore.Mvc;

namespace UAP.Registration.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Status = "Healthy", Service = "UAP.Registration.API" });
    }
}