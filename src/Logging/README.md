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

The `ILogger` extension methods never throw. A null logger or a disabled log level is a no-op, and a missing or blank name, reason or exception is replaced by a placeholder (`UnknownOperation`, `UnknownDependency`, `UnknownResource`, `Unspecified`) so that logging inside a `catch` block can never replace the exception being handled. `LogRetryAttempt` logs the supplied attempt numbers as given.

## Versioning

Stable releases use SemVer package versions derived from `vMAJOR.MINOR.PATCH` release tags.
