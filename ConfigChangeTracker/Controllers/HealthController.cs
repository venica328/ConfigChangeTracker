using Microsoft.AspNetCore.Mvc;

namespace ConfigChangeTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// GET METHOD
        /// Retrieves the health status of the API.
        /// This endpoint can be used for monitoring or automated health checks.
        /// </summary>
        /// <returns>
        /// An object containing the API status ("Healthy") and the current timestamp.
        /// It returns HTTP 200 OK if the API is running properly.
        /// </returns>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Health check requested at {Timestamp}", DateTime.UtcNow);
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
