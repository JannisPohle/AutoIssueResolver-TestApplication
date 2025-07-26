using System;

namespace TestLibrary.S2629.Models
{
    public class WeatherModelBase
    {
        public DateOnly Date { get; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public abstract string Unit { get; }
    }
}