
using System.Globalization;

namespace TestLibrary.S107.Models;

public record WeatherParameters(
    string? Location = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null);
