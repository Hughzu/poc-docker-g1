namespace Docker.WebApi.Data;

public class WeatherDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string WeatherForecastsCollectionName { get; set; } = null!;
}