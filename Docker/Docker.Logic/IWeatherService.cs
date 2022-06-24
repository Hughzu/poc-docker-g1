using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.Domain;

namespace Docker.Logic;

public interface IWeatherService
{
    Task<List<WeatherForecast>> GetAsync();
    Task<WeatherForecast?> GetAsync(int id);
    Task CreateAsync(WeatherForecast newWeatherForecast);
    Task UpdateAsync(int id, WeatherForecast updatedWeatherForecast);
    Task RemoveAsync(int id);
}