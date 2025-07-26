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

  private void Dispose(bool disposing)
  {
    if (disposing)
    {
      _dbManager?.Dispose();
    }
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var cmd = _dbManager.CreateCommand();

    var weatherList = new List<WeatherModelCelsius>();
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

  private sealed class DatabaseManager: IDisposable, IAsyncDisposable
  {
    public void Dispose()
    {
      CloseConnection(_connection);
      _connection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
      if (_connection != null)
      {
        CloseConnection(_connection);
        await _connection.DisposeAsync();
      }
    }

    private SqliteConnection? _connection;

    #region Static

    private const string CONNECTION_STRING = "Data Source=TestFiles/weather.db";

    #endregion

    public async Task<SqliteConnection> InitializeConnection(string? argument)
    {
      _connection ??= new SqliteConnection(argument ?? CONNECTION_STRING);

      if (_connection.State != ConnectionState.Open)
      {
        await _connection.OpenAsync();
      }

      return _connection;
    }

    public SqliteCommand CreateCommand()
    {
      if (_connection is null || _connection.State != ConnectionState.Open)
      {
        throw new InvalidOperationException("Database connection is not open. Call OpenConnection() before accessing the database.");
      }
      var cmd = _connection!.CreateCommand();

      return cmd;
    }

    private static void CloseConnection(SqliteConnection? connection)
    {
      if (connection != null && connection.State != ConnectionState.Closed)
      {
        connection.Close();
      }
    }
  }
}