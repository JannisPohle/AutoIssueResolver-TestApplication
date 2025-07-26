public override Task<List<WeatherModelCelsius>> GetWeather(string? argument)
{
    var weather = GenerateWeatherData(argument);

    if (!weather.Any())
    {
        throw new DataNotFoundException("No weather data available.");
    }

    return Task.FromResult(weather.ToList());
}