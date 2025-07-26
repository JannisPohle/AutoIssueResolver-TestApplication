public sealed class WeatherDbAccessor: WeatherAccessorBase
{
  private readonly string _connectionString;

  public WeatherDbAccessor(ILogger<WeatherDbAccessor> logger, IConfiguration configuration)
    : base(logger)
  {
    _connectionString = configuration.GetConnectionString("DefaultConnection");
  }

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var records = new List<WeatherModelCelsius>();
    
    await using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync();
    
    var query = "SELECT Temperature FROM WeatherData WHERE Location LIKE @Location";
    var command = new SqlCommand(query, connection);

    if (!string.IsNullOrEmpty(argument))
    {
      command.Parameters.AddWithValue("@Location", $"%{argument}%");
    }

    await using var reader = await command.ExecuteReaderAsync();
    
    while (await reader.ReadAsync())
    {
      records.Add(new WeatherModelCelsius((int)reader["Temperature"]));
    }

    return records;
  }
}