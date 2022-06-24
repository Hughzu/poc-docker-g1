using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.WebApi.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Docker.WebApi.Data;

public class WeatherService
{
    private readonly IMongoCollection<WeatherForecast> _weatherForecastsCollection;

    public WeatherService(
        IOptions<WeatherDatabaseSettings> weatherDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            weatherDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            weatherDatabaseSettings.Value.DatabaseName);

        _weatherForecastsCollection = mongoDatabase.GetCollection<WeatherForecast>(
            weatherDatabaseSettings.Value.WeatherForecastsCollectionName);
    }

    public async Task<List<WeatherForecast>> GetAsync() =>
        await _weatherForecastsCollection.Find(_ => true).ToListAsync();

    public async Task<WeatherForecast?> GetAsync(int id) =>
        await _weatherForecastsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(WeatherForecast newWeatherForecast) =>
        await _weatherForecastsCollection.InsertOneAsync(newWeatherForecast);

    public async Task UpdateAsync(int id, WeatherForecast updatedWeatherForecast) =>
        await _weatherForecastsCollection.ReplaceOneAsync(x => x.Id == id, updatedWeatherForecast);

    public async Task RemoveAsync(int id) =>
        await _weatherForecastsCollection.DeleteOneAsync(x => x.Id == id);
}