// <copyright file="LogContextProperty.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>
namespace Atya.Diagnostics.Logging.Context;

/// <summary>
/// Represents one structured logging property.
/// </summary>
public readonly record struct LogContextProperty
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogContextProperty"/> struct.
    /// </summary>
    /// <param name="name">The structured property name.</param>
    /// <param name="value">The structured property value.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, empty, or whitespace.</exception>
    public LogContextProperty(string name, object? value)
    {
        this.Name = Guard.AgainstNullOrWhiteSpace(name);
        this.Value = value;
    }

    /// <summary>
    /// Gets the structured property name.
    /// </summary>
    public string Name
    {
        get;
    }

    /// <summary>
    /// Gets the structured property value.
    /// </summary>
    public object? Value
    {
        get;
    }

    /// <summary>
    /// Converts the current property into a key/value pair.
    /// </summary>
    /// <returns>A key/value representation of the property.</returns>
    public KeyValuePair<string, object?> ToKeyValuePair()
    {
        return new KeyValuePair<string, object?>(this.Name, this.Value);
    }
}
