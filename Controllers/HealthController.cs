using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Health check endpoint for container liveness and readiness probes.
        /// </summary>
        /// <returns>HTTP 200 with status "Healthy" when the application is running.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow.ToString("o")
            });
        }
    }
}
