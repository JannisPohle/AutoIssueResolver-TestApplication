#nullable enable
#language C#
namespace TestLibrary.S1199;

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Dapper; // for database operations if needed, but note that the code uses SqlMapper so we'll keep it as is or adjust accordingly.
using Newtonsoft.Json;

class WeatherAccessorBase
{
    public abstract Task<List<WeatherModelCelsius>> GetWeatherData(string? argument);
}

generator fixer.sarif --version 1.0.0