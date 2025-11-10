using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NodiniteIloggerDemo.FunctionApp.Functions;

public class MyServiceBusTrigger
{
    private readonly ILogger<MyServiceBusTrigger> _logger;

    public MyServiceBusTrigger(ILogger<MyServiceBusTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(MyServiceBusTrigger))]
    public async Task Run(
        [ServiceBusTrigger("nodinite-test", Connection = "servicebuss")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}