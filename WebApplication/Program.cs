private readonly IServiceCollection _services; 

 public WeatherService(IServiceCollection services) 
 { 
 _services = services; 
 } 

 public async Task InitializeAsync() 
 { 
  _services.AddScoped<WeatherOrchestrator, WeatherOrchestrator>(); 
  _services.AddSingleton<AccessMode, AccessMode>(AccessMode.File); 
  _services.AddSingleton<ILogger<WeatherOrchestrator>>(_logger); 
  await _services.BuildServiceProviderAsync(); 
 } 

