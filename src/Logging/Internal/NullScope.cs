// <copyright file="NullScope.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>
namespace Atya.Diagnostics.Logging.Internal;

/// <summary>
/// Represents a no-op disposable scope used when a logger provider returns <see langword="null"/>.
/// </summary>
internal sealed class NullScope : IDisposable
{
    /// <summary>
    /// Gets the singleton no-op scope instance.
    /// </summary>
    public static readonly NullScope Instance = new();

    private NullScope()
    {
    }

    /// <summary>
    /// Performs no work.
    /// </summary>
    public void Dispose()
    {
    }
}
