public async Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null) {
      return await GetWeather(mode, argument, false);
    }

