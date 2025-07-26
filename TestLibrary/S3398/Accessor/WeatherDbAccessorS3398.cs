using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using TestLibrary.S3398.Models;

namespace TestLibrary.S3398.Accessor;

public sealed class WeatherDbAccessor: WeatherAccessorBase, IDisposable
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

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    await OpenConnection(argument);
    return await _dbManager.GetWeather(argument);
  }

  #endregion

  private class DatabaseManager : IDisposable
  {
    public async Task InitializeConnection(string? argument)
    {
      // Implementation for initializing connection
    }

    public async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
    {
      // Implementation for getting weather data
      return new List<WeatherModelCelsius>();
    }

    #region IDisposable Support

    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // TODO: dispose managed state (managed objects).
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.
      }

      disposedValue = true;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion
  }
}
