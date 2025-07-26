private static Result<List<WeatherModelCelsius>> GenerateWeatherData(string? argument) { 
    var random = string.IsNullOrWhiteSpace(argument) ? new Random() : new Random(argument?.GetHashCode() ?? 0); 
 
    if (!int.TryParse(argument, out var count)) { 
        count = 10; // Default count if parsing fails 
    } 
 
    for (var i = 0; i < count; i++) { 
        yield return new WeatherModelCelsius(random.Next(-20, 45)); 
    } 
}