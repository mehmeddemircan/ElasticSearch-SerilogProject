using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch_SerilogProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private static List<WeatherForecast> _weatherForecasts = new List<WeatherForecast>();
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {

            _logger.LogInformation("Hello From action"); 
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

   

        [HttpPost(Name = "CreateWeatherForecast")]
        public IActionResult Create([FromBody] WeatherForecast weatherForecast)
        {
            _logger.LogInformation("Create action called to add a new weather forecast.");

            if (weatherForecast == null)
            {
                _logger.LogWarning("WeatherForecast object was null.");
                return BadRequest("WeatherForecast object cannot be null.");
            }

            weatherForecast.Date = DateOnly.FromDateTime(DateTime.Now);
            _weatherForecasts.Add(weatherForecast);
            _logger.LogInformation("Weather forecast created: {@WeatherForecast}", weatherForecast);

            return CreatedAtAction(nameof(Get), new { id = weatherForecast.Date }, weatherForecast);
        }

        [HttpDelete("{date}", Name = "DeleteWeatherForecast")]
        public IActionResult Delete(DateOnly date)
        {
            _logger.LogInformation("Delete action called for date: {Date}", date);

            var weatherForecast = _weatherForecasts.FirstOrDefault(wf => wf.Date == date);
            if (weatherForecast == null)
            {
                _logger.LogWarning("Weather forecast for date {Date} not found.", date);
                return NotFound();
            }

            _weatherForecasts.Remove(weatherForecast);
            _logger.LogInformation("Weather forecast for date {Date} deleted.", date);

            return NoContent();
        }
    }
}
