using System.Runtime.CompilerServices;
using TestLibrary.S2931;
using FileAccess = TestLibrary.S2930.FileAccess;

[assembly: InternalsVisibleTo("IntegrationTests")]

var builder =Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi()
       .AddLogging(loggingBuilder => loggingBuilder.AddConsole().AddDebug());

builder.Services.AddTransient<FileAccess>();
builder.Services.AddTransient<TestLibrary.S2931.FileAccess>();

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