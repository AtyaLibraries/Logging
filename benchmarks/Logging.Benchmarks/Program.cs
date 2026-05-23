using Atya.Diagnostics.Logging.Context;
using Atya.Diagnostics.Logging.Events;
using Atya.Diagnostics.Logging.Extensions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Logging.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        _ = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class LoggingScopeBenchmarks
{
    private static readonly ILogger s_logger = NullLogger.Instance;

    private static readonly KeyValuePair<string, object?>[] s_keyValueProperties =
    [
        new KeyValuePair<string, object?>(KnownLogPropertyNames.CorrelationId, "corr-123"),
        new KeyValuePair<string, object?>(KnownLogPropertyNames.TenantId, "tenant-1"),
        new KeyValuePair<string, object?>(KnownLogPropertyNames.UserId, "user-1"),
        new KeyValuePair<string, object?>(KnownLogPropertyNames.OperationName, "ProcessOrder")
    ];

    private static readonly LogContextProperty[] s_contextProperties =
    [
        new LogContextProperty(KnownLogPropertyNames.CorrelationId, "corr-123"),
        new LogContextProperty(KnownLogPropertyNames.TenantId, "tenant-1"),
        new LogContextProperty(KnownLogPropertyNames.UserId, "user-1"),
        new LogContextProperty(KnownLogPropertyNames.OperationName, "ProcessOrder")
    ];

    [Benchmark(Baseline = true)]
    public static LogScopeState CreateScopeStateFromKeyValuePairs()
    {
        return new LogScopeState(s_keyValueProperties);
    }

    [Benchmark]
    public static LogScopeState CreateScopeStateFromContextProperties()
    {
        return new LogScopeState(s_contextProperties);
    }

    [Benchmark]
    public static void BeginPropertyScopeFromKeyValuePairs()
    {
        using IDisposable scope = s_logger.BeginPropertyScope(s_keyValueProperties);
    }

    [Benchmark]
    public static void BeginPropertyScopeFromContextProperties()
    {
        using IDisposable scope = s_logger.BeginPropertyScope(s_contextProperties);
    }

    [Benchmark]
    public static void BeginOperationScopeWithCorrelation()
    {
        using IDisposable scope = s_logger.BeginOperationScope("ProcessOrder", "corr-123");
    }
}

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class LoggerMessageBenchmarks
{
    private static readonly ILogger s_logger = NullLogger.Instance;

    private static readonly Action<ILogger, string, object?, Exception?> s_directOperationStarted =
        LoggerMessage.Define<string, object?>(
            LogLevel.Information,
            LoggingEventIds.OperationStarted,
            "Operation {OperationName} started for {Target}.");

    [Benchmark(Baseline = true)]
    public static void DirectLoggerMessageDefine()
    {
        s_directOperationStarted(s_logger, "ProcessOrder", 1001, null);
    }

    [Benchmark]
    public static void LogOperationStartedExtension()
    {
        s_logger.LogOperationStarted("ProcessOrder", 1001);
    }

    [Benchmark]
    public static void LogRetryAttemptExtension()
    {
        s_logger.LogRetryAttempt("ChargePayment", 1, 3);
    }
}
