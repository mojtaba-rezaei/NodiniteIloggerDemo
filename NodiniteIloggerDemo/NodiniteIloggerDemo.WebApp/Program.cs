
using ConticaDevTools.Models.NodiniteILogger;
using ConticaDevTools.Services.NodiniteILogger.Extensions;
using ConticaDevTools.Services.NodiniteILogger.Middleware.WebApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ############# Nodinite config start ################
var connectionString = builder.Configuration.GetValue<string>("connectionString")
    ?? throw new NullReferenceException("Missing connectionString.");

var webbAppName =
    builder.Configuration.GetValue<string>("WEBSITE_CONTENTSHARE")
    ?? AppDomain.CurrentDomain.FriendlyName; // fallback for local dev

var nodiniteStorageAccountName =
    builder.Configuration.GetValue<string>("nodiniteStorageAccountName")
    ?? throw new NullReferenceException("Missing nodiniteStorageAccountName.");

var nodiniteContainerName =
    builder.Configuration.GetValue<string>("nodiniteContainerName")
    ?? throw new NullReferenceException("Missing nodiniteContainerName.");

var settings = new NodiniteSettings()
{
    LogAgentValueId = 15,
    EndPointTypeId = 12,
    EndPointUri = $"https://{nodiniteStorageAccountName}.blob.core.windows.net/{nodiniteContainerName}",
    EndPointDirection = 1,
    ContainerName = nodiniteContainerName,
    ProcessingUser = "NODINITE",
    ProcessName = $"Azure.WebApp.{webbAppName}",
    ProcessingMachineName = "Azure",
    ProcessingModuleName = $"Azure.WebApp.{webbAppName}",
    ProcessingModuleType = "Azure.WebApp",
    ConnectionString = connectionString
};
builder.Services.AddNodiniteLoggingBlobSink(settings, LogLevel.Information);
// ############# Nodinite config end ################

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Nodinite middleware
app.UseMiddleware<LogEnricherMiddleware>();

app.MapControllers();

app.MapGet("/api/test/mojje", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hit this");
    return "ok";
});

app.Run();
