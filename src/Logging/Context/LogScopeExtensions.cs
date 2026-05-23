// <copyright file="LogScopeExtensions.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>
using Atya.Diagnostics.Logging.Internal;

namespace Atya.Diagnostics.Logging.Context;

/// <summary>
/// Provides helpers for starting structured logging scopes.
/// </summary>
public static class LogScopeExtensions
{
    /// <summary>
    /// Begins a scope from the provided name/value tuples.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="properties">The structured properties to include in the scope.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
    public static IDisposable BeginPropertyScope(this ILogger logger, params (string Name, object? Value)[] properties)
    {
        logger = Guard.AgainstNull(logger);
        properties = Guard.AgainstNull(properties);

        var keyValuePairs = new KeyValuePair<string, object?>[properties.Length];
        for (int index = 0; index < properties.Length; index++)
        {
            keyValuePairs[index] = new KeyValuePair<string, object?>(properties[index].Name, properties[index].Value);
        }

        return logger.BeginPropertyScope(keyValuePairs);
    }

    /// <summary>
    /// Begins a scope from the provided structured properties.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="properties">The structured properties to include in the scope.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
    public static IDisposable BeginPropertyScope(this ILogger logger, IEnumerable<KeyValuePair<string, object?>> properties)
    {
        logger = Guard.AgainstNull(logger);
        properties = Guard.AgainstNull(properties);

        LogScopeState state = new LogScopeState(properties);
        return logger.BeginScope(state) ?? NullScope.Instance;
    }

    /// <summary>
    /// Begins a scope from the provided structured properties.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="properties">The structured properties to include in the scope.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
    public static IDisposable BeginPropertyScope(this ILogger logger, IReadOnlyCollection<LogContextProperty> properties)
    {
        logger = Guard.AgainstNull(logger);
        properties = Guard.AgainstNull(properties);

        LogScopeState state = new LogScopeState(properties);
        return logger.BeginScope(state) ?? NullScope.Instance;
    }

    /// <summary>
    /// Begins a scope with a correlation identifier.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
    public static IDisposable BeginCorrelationScope(this ILogger logger, string correlationId)
    {
        return BeginRequiredPropertyScope(
            logger,
            KnownLogPropertyNames.CorrelationId,
            correlationId,
            nameof(correlationId));
    }

    /// <summary>
    /// Begins a scope with a request identifier.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="requestId">The request identifier.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
    public static IDisposable BeginRequestScope(this ILogger logger, string requestId)
    {
        return BeginRequiredPropertyScope(
            logger,
            KnownLogPropertyNames.RequestId,
            requestId,
            nameof(requestId));
    }

    /// <summary>
    /// Begins a scope with a distributed trace identifier.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="traceId">The distributed trace identifier.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
    public static IDisposable BeginTraceScope(this ILogger logger, string traceId)
    {
        return BeginRequiredPropertyScope(
            logger,
            KnownLogPropertyNames.TraceId,
            traceId,
            nameof(traceId));
    }

    /// <summary>
    /// Begins a scope with a user identifier.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
    public static IDisposable BeginUserScope(this ILogger logger, string userId)
    {
        return BeginRequiredPropertyScope(
            logger,
            KnownLogPropertyNames.UserId,
            userId,
            nameof(userId));
    }

    /// <summary>
    /// Begins a scope with a tenant identifier.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
    public static IDisposable BeginTenantScope(this ILogger logger, string tenantId)
    {
        return BeginRequiredPropertyScope(
            logger,
            KnownLogPropertyNames.TenantId,
            tenantId,
            nameof(tenantId));
    }

    /// <summary>
    /// Begins a scope with operation metadata and optional correlation identifier.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="operationName">The operation name.</param>
    /// <param name="correlationId">The optional correlation identifier.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
    public static IDisposable BeginOperationScope(this ILogger logger, string operationName, string? correlationId = null)
    {
        logger = Guard.AgainstNull(logger);
        operationName = Guard.AgainstNullOrWhiteSpace(operationName);

        if (correlationId is null)
        {
            return logger.BeginPropertyScope((KnownLogPropertyNames.OperationName, operationName));
        }

        correlationId = Guard.AgainstNullOrWhiteSpace(correlationId, nameof(correlationId));

        return logger.BeginPropertyScope(
            (KnownLogPropertyNames.OperationName, operationName),
            (KnownLogPropertyNames.CorrelationId, correlationId));
    }

    /// <summary>
    /// Begins a scope with entity metadata and optional operation name.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="operationName">The optional operation name.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
    public static IDisposable BeginEntityScope(this ILogger logger, string entityType, object? entityId, string? operationName = null)
    {
        logger = Guard.AgainstNull(logger);
        entityType = Guard.AgainstNullOrWhiteSpace(entityType);

        if (operationName is not null)
        {
            operationName = Guard.AgainstNullOrWhiteSpace(operationName, nameof(operationName));
        }

        KeyValuePair<string, object?>[] properties = operationName is null
            ?
            [
                new KeyValuePair<string, object?>(KnownLogPropertyNames.EntityType, entityType),
                new KeyValuePair<string, object?>(KnownLogPropertyNames.EntityId, entityId)
            ]
            :
            [
                new KeyValuePair<string, object?>(KnownLogPropertyNames.EntityType, entityType),
                new KeyValuePair<string, object?>(KnownLogPropertyNames.EntityId, entityId),
                new KeyValuePair<string, object?>(KnownLogPropertyNames.OperationName, operationName)
            ];

        return logger.BeginPropertyScope(properties);
    }

    private static IDisposable BeginRequiredPropertyScope(
        ILogger logger,
        string propertyName,
        string value,
        string parameterName)
    {
        logger = Guard.AgainstNull(logger);
        value = Guard.AgainstNullOrWhiteSpace(value, parameterName);

        return logger.BeginPropertyScope((propertyName, value));
    }
}
