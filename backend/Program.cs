using CS2Storage.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Carrega o .env localizado na raiz do projeto
var envPath = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath, "..", ".env")
);

DotNetEnv.Env.Load(envPath);

// Adiciona as variáveis de ambiente à configuração
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<StorageDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
var apiKey = builder.Configuration["ApiKey"];

if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException(
        "ApiKey não configurada no arquivo .env."
    );
}

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        var providedKey = context.Request.Headers["X-API-Key"].ToString();

        var expectedBytes = Encoding.UTF8.GetBytes(apiKey);
        var providedBytes = Encoding.UTF8.GetBytes(providedKey);

        if (!CryptographicOperations.FixedTimeEquals(
            expectedBytes, providedBytes))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API Key inválida ou ausente.");
            return;
        }
    }

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.MapGet("/", () => "CS2 Storage API funcionando!");

app.Run();