using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.Domain;
using Docker.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Docker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherService _weatherService;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        _logger.Log(LogLevel.Information,"Get call");
        return await _weatherService.GetAsync();
    }
    
    [HttpPost(Name = "PostWeatherForecast")]
    public async Task<IActionResult> Post([FromBody] WeatherForecast forecast)
    {
        await _weatherService.CreateAsync(forecast);
        return NoContent();
    }
    
    [HttpPut(Name = "UpdateWeatherForecast/{forecastId}")]
    public async Task<IActionResult> Update([FromQuery]int forecastId, [FromBody] WeatherForecast forecast)
    {
        await _weatherService.UpdateAsync(forecastId, forecast);
        return NoContent();
    }
}