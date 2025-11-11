using ConticaDevTools.Models.NodiniteILogger;
using ConticaDevTools.Services.NodiniteILogger.Extensions;
using ConticaDevTools.Services.NodiniteILogger.Middleware.FunctionApp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// ############# Nodinite config start ################
var connectionString = Environment.GetEnvironmentVariable("connectionString")
    ?? throw new NullReferenceException("Missing connectionString.");

var functionAppName =
    Environment.GetEnvironmentVariable("WEBSITE_CONTENTSHARE")
    ?? AppDomain.CurrentDomain.FriendlyName; // fallback for local dev

var nodiniteStorageAccountName =
    Environment.GetEnvironmentVariable("nodiniteStorageAccountName")
    ?? throw new NullReferenceException("Missing nodiniteStorageAccountName.");

var nodiniteContainerName =
    Environment.GetEnvironmentVariable("nodiniteContainerName")
    ?? throw new NullReferenceException("Missing nodiniteContainerName.");

var settings = new NodiniteSettings()
{
    LogAgentValueId = 15,
    EndPointTypeId = 75,
    EndPointUri = $"https://{nodiniteStorageAccountName}.blob.core.windows.net/{nodiniteContainerName}",
    EndPointDirection = 1,
    ContainerName = nodiniteContainerName,
    ProcessingUser = "NODINITE",
    ProcessName = $"Azure.FunctionApp.{functionAppName}",
    ProcessingMachineName = "Azure",
    ProcessingModuleName = $"Azure.FunctionApp.{functionAppName}",
    ProcessingModuleType = "Azure.FunctionApp",
    ConnectionString = connectionString
};

builder.UseMiddleware<LogEnricherMiddleware>();

builder.Services.AddNodiniteLoggingBlobSink(settings, LogLevel.Information);

// ############# Nodinite config end ################

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
