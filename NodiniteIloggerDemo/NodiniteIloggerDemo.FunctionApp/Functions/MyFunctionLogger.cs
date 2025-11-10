using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace NodiniteIloggerDemo.FunctionApp.Functions;

public class MyFunctionLogger
{
    private readonly ILogger<MyFunctionLogger> _logger;

    public MyFunctionLogger(ILogger<MyFunctionLogger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(MyFunctionLogger))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogWarning($"Function triggered at {DateTime.UtcNow.ToString()}");

        var demoBody = new
        {
            Name = "Simon",
            ClientId = 123456,
            DayOfWeek = DateTime.Now.DayOfWeek,
            Time = DateTime.Now.ToShortTimeString()
        };

        _logger.LogWarning("{LogText} {Body} {ClientId}", "Logtext is here", JsonSerializer.Serialize(demoBody) , demoBody.ClientId);

        return new OkObjectResult("OK");
    }
}