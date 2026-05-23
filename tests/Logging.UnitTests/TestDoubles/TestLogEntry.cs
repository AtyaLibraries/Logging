using Microsoft.Extensions.Logging;

namespace Logging.UnitTests.TestDoubles;

/// <summary>
/// TestLogEntry
/// </summary>
public sealed class TestLogEntry
{
    /// <summary>
    /// LogLevel
    /// </summary>
    public required LogLevel LogLevel
    {
        get; init;
    }

    /// <summary>
    /// EventId
    /// </summary>
    public required EventId EventId
    {
        get; init;
    }

    /// <summary>
    /// Exception
    /// </summary>
    public Exception? Exception
    {
        get; init;
    }

    /// <summary>
    /// Message
    /// </summary>
    public required string Message
    {
        get; init;
    }

    /// <summary>
    /// State
    /// </summary>
    public required IReadOnlyList<KeyValuePair<string, object?>> State
    {
        get; init;
    }
}
