private readonly Mock<WeatherFileAccessor> _fileAccessor = new();
      private readonly Mock<WeatherDbAccessor> _dbAccessor = new();
      private readonly Mock<WeatherApiAccessor> _apiAccessor = new();
      private readonly Mock<WeatherMockAccessor> _mockAccessor = new();