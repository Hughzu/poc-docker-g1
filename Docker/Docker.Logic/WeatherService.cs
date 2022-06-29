using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.Data;
using Docker.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Docker.Logic;

public class WeatherService : IWeatherService
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<WeatherService> _logger;
    
    public WeatherService(IWeatherForecastRepository weatherForecastRepository, IDistributedCache cache, 
        ILogger<WeatherService> logger)
    {
        _weatherForecastRepository = weatherForecastRepository;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<List<WeatherForecast>> GetAsync()
    {
        return await GetFromCache<List<WeatherForecast>>(
            "weatherForecasts",
            "*",
            async () => await _weatherForecastRepository.GetAsync());
    }

    public Task<WeatherForecast?> GetAsync(int id)
    {
        return _weatherForecastRepository.GetAsync(id);
    }

    public Task CreateAsync(WeatherForecast newWeatherForecast)
    {
        _cache.Remove("weatherForecasts:*");
        return _weatherForecastRepository.CreateAsync(newWeatherForecast);
    }

    public Task UpdateAsync(int id, WeatherForecast updatedWeatherForecast)
    {
        _cache.Remove("weatherForecasts:*");
        return _weatherForecastRepository.UpdateAsync(id, updatedWeatherForecast);
    }

    public Task RemoveAsync(int id)
    {
        _cache.Remove("weatherForecasts:*");
        return _weatherForecastRepository.RemoveAsync(id);
    }
    
    private async Task<TResult> GetFromCache<TResult>(string key, string val, Func<Task<object>> func)
    {
        var cacheKey = $"{key}:{val}";
        var data = await _cache.GetStringAsync(cacheKey);

        if (string.IsNullOrEmpty(data))
        {
            _logger.Log(LogLevel.Information,"No cache found");
            data = JsonConvert.SerializeObject(await func());
            await _cache.SetStringAsync(cacheKey, data);
        }
        else
        {
            _logger.Log(LogLevel.Information,"Cache found");
        }

        return JsonConvert.DeserializeObject<TResult>(data);
    }
}