using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.WebApi.Data;
using Docker.WebApi.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Docker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly WeatherService _weatherService;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
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