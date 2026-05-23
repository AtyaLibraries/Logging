// <copyright file="ILogScopeStateFactory.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>
namespace Atya.Diagnostics.Logging.Context;

/// <summary>
/// Creates structured scope state instances.
/// </summary>
public interface ILogScopeStateFactory
{
    /// <summary>
    /// Creates a scope state from the provided structured properties.
    /// </summary>
    /// <param name="properties">The properties that should be included in the scope.</param>
    /// <returns>A normalized immutable scope state.</returns>
    public LogScopeState Create(IEnumerable<KeyValuePair<string, object?>> properties);
}
