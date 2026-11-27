// <copyright file="LoggerExtensions.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>
using Atya.Diagnostics.Logging.Events;

namespace Atya.Diagnostics.Logging.Extensions;

/// <summary>
/// Provides provider-agnostic structured logging helpers.
/// </summary>
public static class LoggerExtensions
{
    private const string UnknownOperation = "UnknownOperation";
    private const string UnknownDependency = "UnknownDependency";
    private const string UnknownResource = "UnknownResource";
    private const string UnknownReason = "Unspecified";

    private static readonly Action<ILogger, string, object?, Exception?> s_operationStartedMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Information,
            LoggingEventIds.OperationStarted,
            "Operation {OperationName} started for {Target}.");

    private static readonly Action<ILogger, string, object?, Exception?> s_operationCompletedMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Information,
            LoggingEventIds.OperationCompleted,
            "Operation {OperationName} completed for {Target}.");

    private static readonly Action<ILogger, string, object?, Exception?> s_operationFailedMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Error,
            LoggingEventIds.OperationFailed,
            "Operation {OperationName} failed for {Target}.");

    private static readonly Action<ILogger, object?, string, Exception?> s_validationFailedMessage =
        LoggerMessage.Define<object?, string>(
            LogLevel.Warning,
            LoggingEventIds.ValidationFailed,
            "Validation failed for {Target}. {Reason}");

    private static readonly Action<ILogger, string, object?, Exception?> s_unhandledExceptionMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Error,
            LoggingEventIds.UnexpectedException,
            "Unhandled exception occurred during {OperationName} for {Target}.");

    private static readonly Action<ILogger, int, int, string, Exception?> s_retryAttemptMessage =
        LoggerMessage.Define<int, int, string>(
            LogLevel.Warning,
            LoggingEventIds.RetryAttempt,
            "Retry attempt {AttemptNumber} of {MaxAttempts} for {OperationName}.");

    private static readonly Action<ILogger, string, object?, Exception?> s_dependencyFailureMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Error,
            LoggingEventIds.DependencyFailure,
            "External dependency {DependencyName} failed for {Target}.");

    private static readonly Action<ILogger, string, object?, Exception?> s_resourceNotFoundMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Information,
            LoggingEventIds.ResourceNotFound,
            "{ResourceType} was not found for {Target}.");

    /// <summary>
    /// Logs the start of an operation.
    /// </summary>
    /// <param name="logger">The logger that writes the message.</param>
    /// <param name="operationName">The operation name associated with the message.</param>
    /// <param name="target">The optional target associated with the operation.</param>
    public static void LogOperationStarted(this ILogger logger, string operationName, object? target = null)
    {
        if (logger is null || !logger.IsEnabled(LogLevel.Information))
        {
            return;
        }

        s_operationStartedMessage(logger, NameOrDefault(operationName, UnknownOperation), target, null);
    }

    /// <summary>
    /// Logs the successful completion of an operation.
    /// </summary>
    /// <param name="logger">The logger that writes the message.</param>
    /// <param name="operationName">The operation name associated with the message.</param>
    /// <param name="target">The optional target associated with the operation.</param>
    public static void LogOperationCompleted(this ILogger logger, string operationName, object? target = null)
    {
        if (logger is null || !logger.IsEnabled(LogLevel.Information))
        {
            return;
        }

        s_operationCompletedMessage(logger, NameOrDefault(operationName, UnknownOperation), target, null);
    }

    /// <summary>
    /// Logs a failed operation together with the thrown exception.
    /// </summary>
    /// <param name="logger">The logger that writes the message.</param>
    /// <param name="exception">The exception that caused the operation to fail.</param>
    /// <param name="operationName">The operation name associated with the failure.</param>
    /// <param name="target">The optional target associated with the operation.</param>
    public static void LogOperationFailed(this ILogger logger, Exception exception, string operationName, object? target = null)
    {
        if (logger is null || !logger.IsEnabled(LogLevel.Error))
        {
            return;
        }

        s_operationFailedMessage(logger, NameOrDefault(operationName, UnknownOperation), target, exception);
    }

    /// <summary>
    /// Logs a validation failure in a structured way.
    /// </summary>
    /// <param name="logger">The logger that writes the message.</param>
    /// <param name="reason">The validation failure reason.</param>
    /// <param name="target">The optional target associated with the validation.</param>
    public static void LogValidationFailed(this ILogger logger, string reason, object? target = null)
    {
        if (logger is null || !logger.IsEnabled(LogLevel.Warning))
        {
            return;
        }

        s_validationFailedMessage(logger, target, NameOrDefault(reason, UnknownReason), null);
    }

    /// <summary>
    /// Logs an unexpected exception in a structured way.
    /// </summary>
    /// <param name="logger">The logger that writes the message.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="operationName">The optional operation name associated with the exception.</param>
    /// <param name="target">The optional target associated with the exception.</param>
    public static void LogUnhandledException(this ILogger logger, Exception exception, string? operationName = null, object? target = null)
    {
        if (logger is null || !logger.IsEnabled(LogLevel.Error))
        {
            return;
        }

        s_unhandledExceptionMessage(logger, NameOrDefault(operationName, UnknownOperation), target, exception);
    }

    /// <summary>
    /// Logs a retry attempt for an operation.
    /// </summary>
    /// <param name="logger">The logger that writes the message.</param>
    /// <param name="operationName">The operation name being retried.</param>
    /// <param name="attemptNumber">The current retry attempt number.</param>
    /// <param name="maxAttempts">The total number of allowed attempts.</param>
    public static void LogRetryAttempt(this ILogger logger, string operationName, int attemptNumber, int maxAttempts)
    {
        if (logger is null || !logger.IsEnabled(LogLevel.Warning))
        {
            return;
        }

        s_retryAttemptMessage(logger, attemptNumber, maxAttempts, NameOrDefault(operationName, UnknownOperation), null);
    }

    /// <summary>
    /// Logs an external dependency failure.
    /// </summary>
    /// <param name="logger">The logger that writes the message.</param>
    /// <param name="exception">The exception raised by the dependency.</param>
    /// <param name="dependencyName">The dependency name.</param>
    /// <param name="target">The optional target associated with the dependency call.</param>
    public static void LogDependencyFailure(this ILogger logger, Exception exception, string dependencyName, object? target = null)
    {
        if (logger is null || !logger.IsEnabled(LogLevel.Error))
        {
            return;
        }

        s_dependencyFailureMessage(logger, NameOrDefault(dependencyName, UnknownDependency), target, exception);
    }

    /// <summary>
    /// Logs that a resource was not found.
    /// </summary>
    /// <param name="logger">The logger that writes the message.</param>
    /// <param name="resourceType">The resource type that could not be found.</param>
    /// <param name="target">The optional target associated with the resource lookup.</param>
    public static void LogResourceNotFound(this ILogger logger, string resourceType, object? target = null)
    {
        if (logger is null || !logger.IsEnabled(LogLevel.Information))
        {
            return;
        }

        s_resourceNotFoundMessage(logger, NameOrDefault(resourceType, UnknownResource), target, null);
    }

    private static string NameOrDefault(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value;
}
