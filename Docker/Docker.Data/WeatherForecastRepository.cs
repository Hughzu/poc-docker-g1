using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Docker.Data;

public class WeatherForecastRepository : IWeatherForecastRepository
{
    private readonly IMongoCollection<WeatherForecastDAO> _weatherForecastsCollection;

    public WeatherForecastRepository(
        IOptions<WeatherDatabaseSettings> weatherDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            weatherDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            weatherDatabaseSettings.Value.DatabaseName);

        _weatherForecastsCollection = mongoDatabase.GetCollection<WeatherForecastDAO>(
            weatherDatabaseSettings.Value.WeatherForecastsCollectionName);
    }

    public async Task<List<WeatherForecast>> GetAsync() {
        var forecasts = await _weatherForecastsCollection.Find(_ => true).ToListAsync();
        return forecasts.Select(Map).ToList();
    }


    public async Task<WeatherForecast?> GetAsync(int id)
    {
        var forecast = await _weatherForecastsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return Map(forecast);
    }
        

    public async Task CreateAsync(WeatherForecast newWeatherForecast) =>
        await _weatherForecastsCollection.InsertOneAsync(Map(newWeatherForecast));

    public async Task UpdateAsync(int id, WeatherForecast updatedWeatherForecast) =>
        await _weatherForecastsCollection.ReplaceOneAsync(x => x.Id == id, Map(updatedWeatherForecast));

    public async Task RemoveAsync(int id) =>
        await _weatherForecastsCollection.DeleteOneAsync(x => x.Id == id);

    private WeatherForecast Map(WeatherForecastDAO forecastDao)
    {
        return new WeatherForecast()
        {
            Id = forecastDao.Id,
            Date = forecastDao.Date,
            Summary = forecastDao.Summary,
            TemperatureC = forecastDao.TemperatureC,
        };
    }
    
    private WeatherForecastDAO Map(WeatherForecast forecastDao)
    {
        return new WeatherForecastDAO()
        {
            Id = forecastDao.Id,
            Date = forecastDao.Date,
            Summary = forecastDao.Summary,
            TemperatureC = forecastDao.TemperatureC,
        };
    }
}