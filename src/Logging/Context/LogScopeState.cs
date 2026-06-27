// <copyright file="LogScopeState.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>
using System.Collections;

namespace Atya.Diagnostics.Logging.Context;

/// <summary>
/// Represents immutable structured scope state that can be passed to <see cref="ILogger.BeginScope{TState}(TState)"/>.
/// </summary>
public sealed class LogScopeState : IReadOnlyList<KeyValuePair<string, object?>>
{
    private static readonly LogScopeState s_empty = new LogScopeState(Array.Empty<KeyValuePair<string, object?>>());

    private readonly KeyValuePair<string, object?>[] _properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogScopeState"/> class.
    /// </summary>
    /// <param name="properties">The structured scope properties.</param>
    public LogScopeState(IEnumerable<KeyValuePair<string, object?>> properties)
    {
        _properties = CreateNormalizedProperties(Guard.AgainstNull(properties));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogScopeState"/> class.
    /// </summary>
    /// <param name="properties">The structured scope properties.</param>
    public LogScopeState(IEnumerable<LogContextProperty> properties)
    {
        _properties = CreateNormalizedProperties(Guard.AgainstNull(properties).Select(static property => property.ToKeyValuePair()));
    }

    /// <summary>
    /// Gets an empty scope state.
    /// </summary>
    public static LogScopeState Empty => s_empty;

    /// <summary>
    /// Gets the number of properties contained in this scope state.
    /// </summary>
    public int Count => _properties.Length;

    /// <summary>
    /// Gets the property at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    /// <returns>The property at the specified index.</returns>
    public KeyValuePair<string, object?> this[int index] => _properties[index];

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, object?>>)_properties).GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _properties.GetEnumerator();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return _properties.Length == 0
            ? string.Empty
            : string.Join(", ", _properties.Select(static x => string.Concat(x.Key, "=", x.Value)));
    }

    private static KeyValuePair<string, object?>[] CreateNormalizedProperties(IEnumerable<KeyValuePair<string, object?>> properties)
    {
        List<KeyValuePair<string, object?>> list = new List<KeyValuePair<string, object?>>();

        foreach (KeyValuePair<string, object?> property in properties)
        {
            if (string.IsNullOrWhiteSpace(property.Key))
            {
                continue;
            }

            list.Add(new KeyValuePair<string, object?>(property.Key, property.Value));
        }

        return list.ToArray();
    }
}
