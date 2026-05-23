// <copyright file="DefaultLogScopeStateFactory.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

namespace Atya.Diagnostics.Logging.Context;

/// <summary>
/// Default implementation of <see cref="ILogScopeStateFactory"/>.
/// </summary>
public sealed class DefaultLogScopeStateFactory : ILogScopeStateFactory
{
    /// <inheritdoc />
    public LogScopeState Create(IEnumerable<KeyValuePair<string, object?>> properties)
    {
        return new LogScopeState(Guard.AgainstNull(properties));
    }
}
