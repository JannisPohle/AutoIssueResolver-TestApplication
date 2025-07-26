{
  if (mode == AccessMode.None) _logger.LogWarning("Access mode is not specified"); return Result<List<WeatherModelCelsius>>.Failure(new ArgumentException("Access mode must be specified", nameof(mode)));
}