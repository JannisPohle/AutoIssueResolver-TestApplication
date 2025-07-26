public sealed class WeatherDbAccessor : WeatherAccessorBase, IDisposable
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