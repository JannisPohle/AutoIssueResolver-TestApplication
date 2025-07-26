        switch (mode)
        {
            case AccessMode.File:
                result = await _fileAccessor.GetWeather(argument);
                break;
            case AccessMode.Mock:
                result = await _mockAccessor.GetWeather(argument);
                break;
            case AccessMode.Database:
                result = await GetWeatherDataFromDbAccessor(argument);
                break;
            case AccessMode.Web:
                result = await _apiAccessor.GetWeather(argument);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }