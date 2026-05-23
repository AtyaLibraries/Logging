using Microsoft.Extensions.Logging;

namespace Logging.UnitTests.TestDoubles;

/// <summary>
/// TestLogger
/// </summary>
public sealed class TestLogger : ILogger
{
    private readonly List<TestLogEntry> _entries = [];
    private readonly List<TestScopeEntry> _scopes = [];

    /// <summary>
    /// Entries
    /// </summary>
    public IReadOnlyList<TestLogEntry> Entries => _entries;

    /// <summary>
    /// Scopes
    /// </summary>
    public IReadOnlyList<TestScopeEntry> Scopes => _scopes;

    /// <summary>
    /// BeginScope
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
    {
        _scopes.Add(new TestScopeEntry { State = state });
        return new TestScope();
    }

    /// <summary>
    /// IsEnabled
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    /// <summary>
    /// Log
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <param name="formatter"></param>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        IReadOnlyList<KeyValuePair<string, object?>> structuredState = state switch
        {
            IReadOnlyList<KeyValuePair<string, object?>> list => list,
            IEnumerable<KeyValuePair<string, object?>> enumerable => enumerable.ToList(),
            _ => []
        };

        _entries.Add(new TestLogEntry
        {
            LogLevel = logLevel,
            EventId = eventId,
            Exception = exception,
            Message = formatter(state, exception),
            State = structuredState
        });
    }

    private sealed class TestScope : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
