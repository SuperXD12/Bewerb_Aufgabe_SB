using Microsoft.AspNetCore.Mvc;

namespace Bewerb_Aufgabe_SB.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpPost("{id}", Name = "GetLocationDetails")]

        public IActionResult GetLocationDetails(int id)
        {
            return Ok(new LocationDetails
            {
                Id = id,
                Name = "Test Location",
                Address = "Test Address",
                City = "Test City",
                State = "Test State",
                ZipCode = "Test ZipCode"
            });
        }

        private class LocationDetails
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Address { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? ZipCode { get; set; }
        }
    }
}