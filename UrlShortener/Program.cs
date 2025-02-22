using UrlShortener.Repositories.Interfaces;
using UrlShortener.Repositories;
using MyUrlShortenerApp.Services;
using UrlShortener.Services.Interface;
using FluentValidation;
using FluentValidation.AspNetCore;
using UrlShortener.Validator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddMemoryCache();
builder.Services.AddValidatorsFromAssemblyContaining<ShortenUrlRequestValidator>();
builder.Services.AddFluentValidationAutoValidation(); // Enables middleware validation

builder.Services.AddSingleton<IUrlRepository, InMemoryUrlRepository>();
builder.Services.AddSingleton<ICacheService, InMemoryCacheService>();
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
