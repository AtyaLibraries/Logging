using Atya.Diagnostics.Logging.Context;
using Logging.UnitTests.TestDoubles;
using Microsoft.Extensions.Logging;

namespace Logging.UnitTests.Context;

public sealed class LogScopeExtensionsTests
{
    [Fact]
    public void BeginPropertyScope_FromTuples_Should_Create_Structured_State()
    {
        var logger = new TestLogger();

        using IDisposable scope = logger.BeginPropertyScope(
            ("CorrelationId", "corr-123"),
            ("TenantId", "tenant-1"));

        LogScopeState state = GetSingleScopeState(logger);
        _ = state.Should().Contain(x => x.Key == "CorrelationId" && Equals(x.Value, "corr-123"));
        _ = state.Should().Contain(x => x.Key == "TenantId" && Equals(x.Value, "tenant-1"));
    }

    [Fact]
    public void BeginPropertyScope_FromEnumerable_Should_Create_Structured_State()
    {
        var logger = new TestLogger();

        using IDisposable scope = logger.BeginPropertyScope(
        [
            new KeyValuePair<string, object?>("UserId", "user-1"),
            new KeyValuePair<string, object?>("Feature", "Checkout")
        ]);

        LogScopeState state = GetSingleScopeState(logger);
        _ = state.Should().Contain(x => x.Key == "UserId" && Equals(x.Value, "user-1"));
        _ = state.Should().Contain(x => x.Key == "Feature" && Equals(x.Value, "Checkout"));
    }

    [Fact]
    public void BeginPropertyScope_FromContextProperties_Should_Create_Structured_State()
    {
        var logger = new TestLogger();
        LogContextProperty[] properties =
        [
            new(KnownLogPropertyNames.TraceId, "trace-1"),
            new(KnownLogPropertyNames.RequestId, "request-1")
        ];

        using IDisposable scope = logger.BeginPropertyScope(properties);

        LogScopeState state = GetSingleScopeState(logger);
        _ = state.Should().Contain(x => x.Key == KnownLogPropertyNames.TraceId && Equals(x.Value, "trace-1"));
        _ = state.Should().Contain(x => x.Key == KnownLogPropertyNames.RequestId && Equals(x.Value, "request-1"));
    }

    [Fact]
    public void BeginPropertyScope_Should_Return_NullScope_When_Logger_Returns_Null()
    {
        var logger = new NullScopeLogger();

        IDisposable scope = logger.BeginPropertyScope(
        [
            new KeyValuePair<string, object?>("A", 1)
        ]);

        scope.Dispose();

        _ = logger.State.Should().BeOfType<LogScopeState>();
    }

    [Fact]
    public void BeginPropertyScope_FromContextProperties_Should_Return_NullScope_When_Logger_Returns_Null()
    {
        var logger = new NullScopeLogger();
        LogContextProperty[] properties =
        [
            new("A", 1)
        ];

        IDisposable scope = logger.BeginPropertyScope(properties);

        scope.Dispose();

        _ = logger.State.Should().BeOfType<LogScopeState>();
    }

    [Theory]
    [InlineData(nameof(KnownLogPropertyNames.CorrelationId), KnownLogPropertyNames.CorrelationId, "corr-123")]
    [InlineData(nameof(KnownLogPropertyNames.RequestId), KnownLogPropertyNames.RequestId, "request-123")]
    [InlineData(nameof(KnownLogPropertyNames.TraceId), KnownLogPropertyNames.TraceId, "trace-123")]
    [InlineData(nameof(KnownLogPropertyNames.UserId), KnownLogPropertyNames.UserId, "user-123")]
    [InlineData(nameof(KnownLogPropertyNames.TenantId), KnownLogPropertyNames.TenantId, "tenant-123")]
    public void BeginNamedScope_Should_Include_Expected_Property(string methodName, string propertyName, string propertyValue)
    {
        var logger = new TestLogger();

        using IDisposable scope = methodName switch
        {
            nameof(KnownLogPropertyNames.CorrelationId) => logger.BeginCorrelationScope(propertyValue),
            nameof(KnownLogPropertyNames.RequestId) => logger.BeginRequestScope(propertyValue),
            nameof(KnownLogPropertyNames.TraceId) => logger.BeginTraceScope(propertyValue),
            nameof(KnownLogPropertyNames.UserId) => logger.BeginUserScope(propertyValue),
            nameof(KnownLogPropertyNames.TenantId) => logger.BeginTenantScope(propertyValue),
            _ => throw new InvalidOperationException("Unexpected method name.")
        };

        LogScopeState state = GetSingleScopeState(logger);
        _ = state.Should().ContainSingle(x => x.Key == propertyName && Equals(x.Value, propertyValue));
    }

    [Fact]
    public void BeginOperationScope_Should_Include_Only_OperationName_When_CorrelationId_Is_Not_Provided()
    {
        var logger = new TestLogger();

        using IDisposable scope = logger.BeginOperationScope("ProcessOrder");

        LogScopeState state = GetSingleScopeState(logger);
        _ = state.Should().ContainSingle(x => x.Key == KnownLogPropertyNames.OperationName && Equals(x.Value, "ProcessOrder"));
    }

    [Fact]
    public void BeginOperationScope_Should_Include_OperationName_And_CorrelationId()
    {
        var logger = new TestLogger();

        using IDisposable scope = logger.BeginOperationScope("ProcessOrder", "corr-123");

        LogScopeState state = GetSingleScopeState(logger);
        _ = state.Should().Contain(x => x.Key == KnownLogPropertyNames.OperationName && Equals(x.Value, "ProcessOrder"));
        _ = state.Should().Contain(x => x.Key == KnownLogPropertyNames.CorrelationId && Equals(x.Value, "corr-123"));
    }

    [Fact]
    public void BeginEntityScope_Should_Include_EntityType_And_EntityId()
    {
        var logger = new TestLogger();

        using IDisposable scope = logger.BeginEntityScope("Order", 42);

        LogScopeState state = GetSingleScopeState(logger);
        _ = state.Should().Contain(x => x.Key == KnownLogPropertyNames.EntityType && Equals(x.Value, "Order"));
        _ = state.Should().Contain(x => x.Key == KnownLogPropertyNames.EntityId && Equals(x.Value, 42));
        _ = state.Should().NotContain(x => x.Key == KnownLogPropertyNames.OperationName);
    }

    [Fact]
    public void BeginEntityScope_Should_Include_EntityType_EntityId_And_OperationName()
    {
        var logger = new TestLogger();

        using IDisposable scope = logger.BeginEntityScope("Order", 42, "CreateOrder");

        LogScopeState state = GetSingleScopeState(logger);
        _ = state.Should().Contain(x => x.Key == KnownLogPropertyNames.EntityType && Equals(x.Value, "Order"));
        _ = state.Should().Contain(x => x.Key == KnownLogPropertyNames.EntityId && Equals(x.Value, 42));
        _ = state.Should().Contain(x => x.Key == KnownLogPropertyNames.OperationName && Equals(x.Value, "CreateOrder"));
    }

    [Fact]
    public void BeginPropertyScope_FromTuples_Should_Throw_When_Logger_Is_Null()
    {
        ILogger logger = null!;

        Action action = () => _ = logger.BeginPropertyScope(("CorrelationId", "corr-1"));

        _ = action.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName == "logger");
    }

    [Fact]
    public void BeginPropertyScope_FromTuples_Should_Throw_When_Properties_Is_Null()
    {
        var logger = new TestLogger();
        (string Name, object? Value)[] properties = null!;

        Action action = () => _ = logger.BeginPropertyScope(properties);

        _ = action.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName == "properties");
    }

    [Fact]
    public void BeginPropertyScope_FromEnumerable_Should_Throw_When_Properties_Is_Null()
    {
        var logger = new TestLogger();
        IEnumerable<KeyValuePair<string, object?>> properties = null!;

        Action action = () => _ = logger.BeginPropertyScope(properties);

        _ = action.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName == "properties");
    }

    [Fact]
    public void BeginPropertyScope_FromContextProperties_Should_Throw_When_Properties_Is_Null()
    {
        var logger = new TestLogger();
        IReadOnlyCollection<LogContextProperty> properties = null!;

        Action action = () => _ = logger.BeginPropertyScope(properties);

        _ = action.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName == "properties");
    }

    [Fact]
    public void BeginUserScope_Should_Throw_When_Logger_Is_Null()
    {
        ILogger logger = null!;

        Action action = () => _ = logger.BeginUserScope("user-1");

        _ = action.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName == "logger");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void BeginRequestScope_Should_Throw_When_RequestId_Is_Invalid(string? requestId)
    {
        var logger = new TestLogger();

        Action action = () => _ = logger.BeginRequestScope(requestId!);

        _ = action.Should().Throw<ArgumentException>().Where(exception => exception.ParamName == "requestId");
    }

    [Fact]
    public void BeginOperationScope_Should_Throw_When_OperationName_Is_Invalid()
    {
        var logger = new TestLogger();

        Action action = () => _ = logger.BeginOperationScope(" ");

        _ = action.Should().Throw<ArgumentException>().Where(exception => exception.ParamName == "operationName");
    }

    [Fact]
    public void BeginOperationScope_Should_Throw_When_CorrelationId_Is_Invalid()
    {
        var logger = new TestLogger();

        Action action = () => _ = logger.BeginOperationScope("ProcessOrder", " ");

        _ = action.Should().Throw<ArgumentException>().Where(exception => exception.ParamName == "correlationId");
    }

    [Fact]
    public void BeginEntityScope_Should_Throw_When_EntityType_Is_Invalid()
    {
        var logger = new TestLogger();

        Action action = () => _ = logger.BeginEntityScope(" ", 1);

        _ = action.Should().Throw<ArgumentException>().Where(exception => exception.ParamName == "entityType");
    }

    [Fact]
    public void BeginEntityScope_Should_Throw_When_OperationName_Is_Invalid()
    {
        var logger = new TestLogger();

        Action action = () => _ = logger.BeginEntityScope("Order", 1, " ");

        _ = action.Should().Throw<ArgumentException>().Where(exception => exception.ParamName == "operationName");
    }

    private static LogScopeState GetSingleScopeState(TestLogger logger)
    {
        _ = logger.Scopes.Should().ContainSingle();
        return logger.Scopes[0].State.Should().BeOfType<LogScopeState>().Subject;
    }

    private sealed class NullScopeLogger : ILogger
    {
        public object? State
        {
            get;
            private set;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            this.State = state;
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            _ = logLevel;
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            _ = logLevel;
            _ = eventId;
            _ = state;
            _ = exception;
            _ = formatter;
        }
    }
}
