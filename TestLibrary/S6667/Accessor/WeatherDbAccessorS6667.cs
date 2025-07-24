using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using TestLibrary.S6667.Models;

namespace TestLibrary.S6667.Accessor;

public sealed class WeatherDbAccessor: WeatherAccessorBase, IDisposable
{
  #region Static

  private const string CONNECTION_STRING = "Data Source=TestFiles/weather.db";

  #endregion

  #region Members

  private SqliteConnection? _connection;

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
      _connection ??= new SqliteConnection(argument ?? CONNECTION_STRING);

      if (_connection.State != ConnectionState.Open)
      {
        await _connection.OpenAsync();
      }
    }
    catch (Exception e)
    {
      Logger.LogError(e, "Failed to open database connection with argument: {Argument}", argument);

      throw new ConnectionFailedException("Failed to open the database connection. Please check the connection string or database availability.", e);
    }
  }

  private void CloseConnection()
  {
    if (_connection != null && _connection.State != ConnectionState.Closed)
    {
      _connection.Close();
    }
  }

  private void Dispose(bool disposing)
  {
    if (disposing)
    {
      CloseConnection();
      _connection?.Dispose();
    }
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    if (_connection is null || _connection.State != ConnectionState.Open)
    {
      throw new InvalidOperationException("Database connection is not open. Call OpenConnection() before accessing the database.");
    }

    var weatherList = new List<WeatherModelCelsius>();
    var cmd = _connection!.CreateCommand();
    cmd.CommandText = "SELECT Temperature FROM Weather";

    await using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
      var temperature = reader.GetInt32(0);
      weatherList.Add(new WeatherModelCelsius(temperature));
    }

    return weatherList;
  }

  #endregion
}
