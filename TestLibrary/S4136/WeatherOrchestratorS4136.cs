        public async Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null)
        {
          try
          {
            if (mode == AccessMode.None)
            {
              return Result<List<WeatherModelCelsius>>.Failure(new ArgumentException("Access mode must be specified", nameof(mode)));
            }
             
            _logger.LogInformation("Getting weather from {AccessMode} with Argument: {Argument}", mode, argument);
             
            var result = mode switch
            {
              AccessMode.File => await _fileAccessor.GetWeather(argument),
              AccessMode.Mock => await _mockAccessor.GetWeather(argument),
              AccessMode.Database => await GetWeatherDataFromDbAccessor(argument),
              AccessMode.Web => await _apiAccessor.GetWeather(argument),
              _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
             
            _logger.LogInformation("Retrieved {Count} weather records", result.Count);
             
            return Result<List<WeatherModelCelsius>>.Success(result);
          }
          catch (Exception e)
          {
            _logger.LogError(e, "Error retrieving weather data");
             
            return Result<List<WeatherModelCelsius>>.Failure(e);
          }
        }
         
        public async Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument, bool throwOnError)
        {
          var result = await GetWeather(mode, argument);
           
          if (!throwOnError || result.IsSuccess)
          {
            return result;
          }
           
          throw result.Exception!;
        }