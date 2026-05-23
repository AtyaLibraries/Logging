# Atya.Diagnostics.Logging

Provider-agnostic structured logging helpers for .NET applications.

## Installation

```bash
dotnet add package Atya.Diagnostics.Logging
```

## Target framework

This package targets `net10.0`.

## What this package provides

- Structured scope helpers for correlation, request, trace, user, tenant, operation, entity, and custom properties.
- Common logging event identifiers for operation, validation, retry, dependency, missing resource, and unexpected exception events.
- `LoggerMessage.Define`-based extension methods for common structured log messages.
- Dependency injection registration for package-owned logging services.

This package builds on `Microsoft.Extensions.Logging`. It does not configure a concrete logging provider, OpenTelemetry exporter, sink, or storage integration.

## Quick start

```csharp
using Atya.Diagnostics.Logging.Context;
using Atya.Diagnostics.Logging.DependencyInjection;
using Atya.Diagnostics.Logging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddLogging(builder => builder.AddConsole());
services.AddAtyaLogging();

using ServiceProvider provider = services.BuildServiceProvider();
ILogger<Program> logger = provider.GetRequiredService<ILogger<Program>>();

using IDisposable correlationScope = logger.BeginCorrelationScope("corr-123");
using IDisposable tenantScope = logger.BeginTenantScope("tenant-1");
using IDisposable operationScope = logger.BeginOperationScope("ProcessOrder", "corr-123");

logger.LogOperationStarted("ProcessOrder", 1001);

try
{
    logger.LogRetryAttempt("ChargePayment", attemptNumber: 1, maxAttempts: 3);
    logger.LogOperationCompleted("ProcessOrder", 1001);
}
catch (Exception exception)
{
    logger.LogDependencyFailure(exception, "PaymentGateway", 1001);
    logger.LogOperationFailed(exception, "ProcessOrder", 1001);
}
```

## Custom scopes

```csharp
using Atya.Diagnostics.Logging.Context;

using IDisposable scope = logger.BeginPropertyScope(
    new[]
    {
        new LogContextProperty(KnownLogPropertyNames.UserId, "user-1"),
        new LogContextProperty("Feature", "Checkout")
    });
```

Blank property names are ignored when a `LogScopeState` is created from key/value pairs. Named scope helpers throw for null, empty, or whitespace identifiers.

## Exceptions

Argument validation is implemented with `Atya.Foundation.Guards`. Most extension methods throw `ArgumentNullException` for a null `ILogger`, required exception argument, or required string argument. Methods that require names or identifiers throw `ArgumentException` when those values are empty or whitespace. `LogRetryAttempt` throws `ArgumentOutOfRangeException` when attempt numbers are less than one or the current attempt is greater than the maximum attempt count.

## Versioning

Stable releases use SemVer package versions derived from `vMAJOR.MINOR.PATCH` release tags.
