// Simplify and refactor the GetWeather method by using pattern matching
// and reducing nested conditionals.

public async Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null)
{
    if (mode == AccessMode.None)
    {
        return Result<List<WeatherModelCelsius>>.Failure(new ArgumentException("Access mode must be specified", nameof(mode)));
    }

    try
    {
        _logger.LogInformation("Getting weather from {AccessMode} with Argument: {Argument}", mode, argument);

        List<WeatherModelCelsius> result = await GetWeatherDataAsync(mode, argument);

        ValidateWeatherData(result);

        _logger.LogInformation("Retrieved {Count} weather records", result.Count);

        return Result<List<WeatherModelCelsius>>.Success(result);
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Error retrieving weather data");
        return Result<List<WeatherModelCelsius>>.Failure(e);
    }
}

private async Task<List<WeatherModelCelsius>> GetWeatherDataAsync(AccessMode mode, string? argument)
{
    var accessor = mode switch
    {
        AccessMode.File => _fileAccessor,
        AccessMode.Database => await _dbAccessor.OpenConnection(argument),
        AccessMode.Web => _apiAccessor,
        AccessMode.Mock => _mockAccessor,
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
    };

    return accessor.GetWeatherData(argument).Result;
}

private void ValidateWeatherData(List<WeatherModelCelsius> result)
{
    foreach (var weather in result)
    {
        if (weather.Temperature > 120 || weather.Temperature < -100)
        {
            throw new ValidationException("Temperature must be between -100 and 120 degrees Celsius");
        }

        if (string.IsNullOrWhiteSpace(weather.Unit))
        {
            throw new ValidationException("Unit cannot be null or empty");
        }
    }
}