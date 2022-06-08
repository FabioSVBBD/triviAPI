using Microsoft.AspNetCore.Mvc;

namespace QuizAPI.Controllers
{
    [ApiController]
    [Route("/test/api")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "test")]
        public IActionResult testMe()
        {
					return new ObjectResult("hello world");
        }
    }
}