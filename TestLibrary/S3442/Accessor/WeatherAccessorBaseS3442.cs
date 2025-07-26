protected WeatherAccessorBase(ILogger<WeatherAccessorBase> logger){
    Logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }