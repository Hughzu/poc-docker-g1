using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Docker.WebApi.Domain;

public class WeatherForecast
{
    [BsonId]
    [BsonRepresentation(BsonType.Int64)]
    public int? Id { get; set; }
    public string Summary { get; set; } = null!;
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}