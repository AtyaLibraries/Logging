using System.Diagnostics;
using Atya.Diagnostics.Logging.Context;
using Atya.Diagnostics.Logging.DependencyInjection;
using Atya.Diagnostics.Logging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    _ = builder.ClearProviders();
    _ = builder.AddSimpleConsole(options =>
    {
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
    });
    _ = builder.SetMinimumLevel(LogLevel.Information);
});

services.AddAtyaLogging();
services.AddTransient<OrderProcessor>();

using ServiceProvider serviceProvider = services.BuildServiceProvider();
OrderProcessor processor = serviceProvider.GetRequiredService<OrderProcessor>();

await processor.ProcessAsync(orderId: 1001, correlationId: Guid.NewGuid().ToString("N"));

internal sealed class OrderProcessor(ILogger<OrderProcessor> logger)
{
    private static readonly Action<ILogger, Exception?> s_loadingOrderDataMessage =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(2000, "LoadingOrderData"),
            "Loading order data.");

    public async Task ProcessAsync(int orderId, string correlationId)
    {
        using IDisposable correlationScope = logger.BeginCorrelationScope(correlationId);
        using IDisposable requestScope = logger.BeginRequestScope($"request-{orderId}");
        using IDisposable traceScope = logger.BeginTraceScope(ActivityTraceId.CreateRandom().ToString());
        using IDisposable userScope = logger.BeginUserScope("system-user");
        using IDisposable tenantScope = logger.BeginTenantScope("default");
        using IDisposable operationScope = logger.BeginOperationScope("ProcessOrder", correlationId);
        using IDisposable entityScope = logger.BeginEntityScope("Order", orderId, "ProcessOrder");

        logger.LogOperationStarted("ProcessOrder", orderId);

        try
        {
            using IDisposable propertyScope = logger.BeginPropertyScope(
                ("Feature", "Checkout"));

            s_loadingOrderDataMessage(logger, null);
            await Task.Delay(100);

            logger.LogRetryAttempt("ChargePayment", 1, 3);
            await Task.Delay(100);

            throw new InvalidOperationException("Payment gateway rejected the request.");
        }
        catch (Exception exception)
        {
            logger.LogDependencyFailure(exception, "PaymentGateway", orderId);
            logger.LogOperationFailed(exception, "ProcessOrder", orderId);
            logger.LogUnhandledException(exception, "ProcessOrder", orderId);
        }
        finally
        {
            logger.LogOperationCompleted("ProcessOrder", orderId);
        }
    }
}
