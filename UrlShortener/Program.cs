using UrlShortener.Repositories.Interfaces;
using UrlShortener.Repositories;
using MyUrlShortenerApp.Services;
using UrlShortener.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddLogging();

builder.Services.AddSingleton<IUrlRepository, InMemoryUrlRepository>(); // Registering the repository
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
