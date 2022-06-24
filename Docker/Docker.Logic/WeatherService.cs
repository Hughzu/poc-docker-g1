using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.Data;
using Docker.Domain;

namespace Docker.Logic;

public class WeatherService : IWeatherService
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;
    
    public WeatherService(IWeatherForecastRepository weatherForecastRepository)
    {
        _weatherForecastRepository = weatherForecastRepository;
    }
    
    public Task<List<WeatherForecast>> GetAsync()
    {
        return _weatherForecastRepository.GetAsync();
    }

    public Task<WeatherForecast?> GetAsync(int id)
    {
        return _weatherForecastRepository.GetAsync(id);
    }

    public Task CreateAsync(WeatherForecast newWeatherForecast)
    {
        return _weatherForecastRepository.CreateAsync(newWeatherForecast);
    }

    public Task UpdateAsync(int id, WeatherForecast updatedWeatherForecast)
    {
        return _weatherForecastRepository.UpdateAsync(id, updatedWeatherForecast);
    }

    public Task RemoveAsync(int id)
    {
        return _weatherForecastRepository.RemoveAsync(id);
    }
}