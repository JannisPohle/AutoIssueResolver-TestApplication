using System.Runtime.CompilerServices;
using TestLibrary.Template;
using TestLibrary.Template.Abstractions;
using TestLibrary.Template.Accessor;

[assembly: InternalsVisibleTo("IntegrationTests")]

var builder =Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi()
       .AddLogging(loggingBuilder => loggingBuilder.AddConsole().AddDebug());

builder.Services.AddTransient<IWeatherOrchestrator, WeatherOrchestrator>()
       .AddTransient<WeatherMockAccessor>()
       .AddTransient<WeatherDbAccessor>()
       .AddTransient<WeatherApiAccessor>()
       .AddTransient<WeatherFileAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();