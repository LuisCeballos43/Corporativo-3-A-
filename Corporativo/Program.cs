using Corporativo.Data;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

Env.Load();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var mySecret = Environment.GetEnvironmentVariable("ConectionToDatabase");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configuración para que el JSON sea case-insensitive
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // Permite números como strings
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        // IMPORTANTE: Maneja ciclos de referencia
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Configura para leer propiedades adicionales
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });

var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddDbContext<CorporativoContext>(options =>
 options.UseSqlServer(mySecret));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); 
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
