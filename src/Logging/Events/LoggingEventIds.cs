// <copyright file="LoggingEventIds.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>
namespace Atya.Diagnostics.Logging.Events;

/// <summary>
/// Common reusable event identifiers used by this package.
/// </summary>
public static class LoggingEventIds
{
    /// <summary>
    /// Gets the event identifier used when an operation starts.
    /// </summary>
    public static readonly EventId OperationStarted = new(1000, nameof(OperationStarted));

    /// <summary>
    /// Gets the event identifier used when an operation completes successfully.
    /// </summary>
    public static readonly EventId OperationCompleted = new(1001, nameof(OperationCompleted));

    /// <summary>
    /// Gets the event identifier used when an operation fails.
    /// </summary>
    public static readonly EventId OperationFailed = new(1002, nameof(OperationFailed));

    /// <summary>
    /// Gets the event identifier used for validation failures.
    /// </summary>
    public static readonly EventId ValidationFailed = new(1100, nameof(ValidationFailed));

    /// <summary>
    /// Gets the event identifier used for retry attempts.
    /// </summary>
    public static readonly EventId RetryAttempt = new(1200, nameof(RetryAttempt));

    /// <summary>
    /// Gets the event identifier used for dependency failures.
    /// </summary>
    public static readonly EventId DependencyFailure = new(1300, nameof(DependencyFailure));

    /// <summary>
    /// Gets the event identifier used when a resource cannot be found.
    /// </summary>
    public static readonly EventId ResourceNotFound = new(1400, nameof(ResourceNotFound));

    /// <summary>
    /// Gets the event identifier used for unexpected exceptions.
    /// </summary>
    public static readonly EventId UnexpectedException = new(1500, nameof(UnexpectedException));
}
