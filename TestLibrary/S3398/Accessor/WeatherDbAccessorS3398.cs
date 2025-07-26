public class WeatherDbAccessor: WeatherAccessorBase, IDisposable
{
  #region Members
  
  private readonly DatabaseManager _dbManager = new();
  
  #endregion
  
  #region Constructors
  
  public WeatherDbAccessor(ILogger<WeatherDbAccessor> logger)
    : base(logger)
  {
    SQLitePCL.Batteries_V2.Init();
  }
  
  #endregion
  
  #region Methods
  
  public async Task OpenConnection(string? argument)
  {
    try
    {
      await _dbManager.InitializeConnection(argument);
    }
    catch (Exception e)
    {
      Logger.LogError(e, "Failed to open database connection with argument: {Argument}", argument);

      throw new ConnectionFailedException("Failed to open the database connection. Please check the connection string or database availability.", e);
    }
  }

  private async Task<List<WeatherModelCelsius>> GetWeatherDataFromDbAccessor(string? argument)
  {
    await _dbAccessor.OpenConnection(argument);
    return await _dbAccessor.GetWeather(argument);
  }

  #endregion

  
  public class DatabaseManager
  {
    public async Task InitializeConnection(string? argument)
    {
      // Implementation of InitializeConnection
    }

    private static void CloseDatabase()
    {
      // Implementation of CloseDatabase
    }
  }