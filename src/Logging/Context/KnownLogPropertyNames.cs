// <copyright file="KnownLogPropertyNames.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>
namespace Atya.Diagnostics.Logging.Context;

/// <summary>
/// Provides well-known structured log property names used by this package.
/// </summary>
public static class KnownLogPropertyNames
{
    /// <summary>
    /// Gets the property name used for correlation identifiers.
    /// </summary>
    public const string CorrelationId = "CorrelationId";

    /// <summary>
    /// Gets the property name used for request identifiers.
    /// </summary>
    public const string RequestId = "RequestId";

    /// <summary>
    /// Gets the property name used for distributed trace identifiers.
    /// </summary>
    public const string TraceId = "TraceId";

    /// <summary>
    /// Gets the property name used for user identifiers.
    /// </summary>
    public const string UserId = "UserId";

    /// <summary>
    /// Gets the property name used for tenant identifiers.
    /// </summary>
    public const string TenantId = "TenantId";

    /// <summary>
    /// Gets the property name used for operation names.
    /// </summary>
    public const string OperationName = "OperationName";

    /// <summary>
    /// Gets the property name used for entity type names.
    /// </summary>
    public const string EntityType = "EntityType";

    /// <summary>
    /// Gets the property name used for entity identifiers.
    /// </summary>
    public const string EntityId = "EntityId";
}
