using Atya.Diagnostics.Logging.Events;
using Atya.Diagnostics.Logging.Extensions;
using Logging.UnitTests.TestDoubles;
using Microsoft.Extensions.Logging;

namespace Logging.UnitTests.Extensions;

public sealed class LoggerExtensionsTests
{
    [Fact]
    public void LogOperationStarted_Should_Write_Information_Log()
    {
        var logger = new TestLogger();

        logger.LogOperationStarted("ProcessOrder", 123);

        TestLogEntry entry = logger.Entries.Should().ContainSingle().Subject;
        _ = entry.LogLevel.Should().Be(LogLevel.Information);
        _ = entry.EventId.Should().Be(LoggingEventIds.OperationStarted);
        _ = entry.Message.Should().Contain("ProcessOrder").And.Contain("123");
        _ = entry.State.Should().Contain(x => x.Key == "OperationName" && Equals(x.Value, "ProcessOrder"));
        _ = entry.State.Should().Contain(x => x.Key == "Target" && Equals(x.Value, 123));
    }

    [Fact]
    public void LogOperationCompleted_Should_Write_Information_Log()
    {
        var logger = new TestLogger();

        logger.LogOperationCompleted("ProcessOrder", 123);

        TestLogEntry entry = logger.Entries.Should().ContainSingle().Subject;
        _ = entry.LogLevel.Should().Be(LogLevel.Information);
        _ = entry.EventId.Should().Be(LoggingEventIds.OperationCompleted);
    }

    [Fact]
    public void LogOperationFailed_Should_Write_Error_Log_With_Exception()
    {
        var logger = new TestLogger();
        var exception = new InvalidOperationException("boom");

        logger.LogOperationFailed(exception, "ProcessOrder", 123);

        TestLogEntry entry = logger.Entries.Should().ContainSingle().Subject;
        _ = entry.LogLevel.Should().Be(LogLevel.Error);
        _ = entry.EventId.Should().Be(LoggingEventIds.OperationFailed);
        _ = entry.Exception.Should().BeSameAs(exception);
        _ = entry.State.Should().Contain(x => x.Key == "OperationName" && Equals(x.Value, "ProcessOrder"));
        _ = entry.State.Should().Contain(x => x.Key == "Target" && Equals(x.Value, 123));
    }

    [Fact]
    public void LogValidationFailed_Should_Write_Warning_Log()
    {
        var logger = new TestLogger();

        logger.LogValidationFailed("Input validation failed.", "command-1");

        TestLogEntry entry = logger.Entries.Should().ContainSingle().Subject;
        _ = entry.LogLevel.Should().Be(LogLevel.Warning);
        _ = entry.EventId.Should().Be(LoggingEventIds.ValidationFailed);
        _ = entry.State.Should().Contain(x => x.Key == "Reason" && Equals(x.Value, "Input validation failed."));
        _ = entry.State.Should().Contain(x => x.Key == "Target" && Equals(x.Value, "command-1"));
    }

    [Fact]
    public void LogUnhandledException_Should_Write_Error_Log_With_Default_Operation_Name_When_Missing()
    {
        var logger = new TestLogger();
        var exception = new InvalidOperationException("unexpected");

        logger.LogUnhandledException(exception);

        TestLogEntry entry = logger.Entries.Should().ContainSingle().Subject;
        _ = entry.LogLevel.Should().Be(LogLevel.Error);
        _ = entry.EventId.Should().Be(LoggingEventIds.UnexpectedException);
        _ = entry.Exception.Should().BeSameAs(exception);
        _ = entry.State.Should().Contain(x => x.Key == "OperationName" && Equals(x.Value, "UnknownOperation"));
    }

    [Fact]
    public void LogUnhandledException_Should_Write_Error_Log_With_Provided_Operation_Name()
    {
        var logger = new TestLogger();
        var exception = new InvalidOperationException("unexpected");

        logger.LogUnhandledException(exception, "ProcessOrder", 123);

        TestLogEntry entry = logger.Entries.Should().ContainSingle().Subject;
        _ = entry.LogLevel.Should().Be(LogLevel.Error);
        _ = entry.EventId.Should().Be(LoggingEventIds.UnexpectedException);
        _ = entry.Exception.Should().BeSameAs(exception);
        _ = entry.State.Should().Contain(x => x.Key == "OperationName" && Equals(x.Value, "ProcessOrder"));
        _ = entry.State.Should().Contain(x => x.Key == "Target" && Equals(x.Value, 123));
    }

    [Fact]
    public void LogUnhandledException_Should_Use_Default_Operation_Name_When_Provided_Value_Is_Whitespace()
    {
        var logger = new TestLogger();
        var exception = new InvalidOperationException("unexpected");

        logger.LogUnhandledException(exception, " ");

        TestLogEntry entry = logger.Entries.Should().ContainSingle().Subject;
        _ = entry.State.Should().Contain(x => x.Key == "OperationName" && Equals(x.Value, "UnknownOperation"));
    }

    [Fact]
    public void LogRetryAttempt_Should_Write_Warning_Log()
    {
        var logger = new TestLogger();

        logger.LogRetryAttempt("ImportCustomers", 2, 5);

        TestLogEntry entry = logger.Entries.Should().ContainSingle().Subject;
        _ = entry.LogLevel.Should().Be(LogLevel.Warning);
        _ = entry.EventId.Should().Be(LoggingEventIds.RetryAttempt);
        _ = entry.State.Should().Contain(x => x.Key == "AttemptNumber" && Equals(x.Value, 2));
        _ = entry.State.Should().Contain(x => x.Key == "MaxAttempts" && Equals(x.Value, 5));
        _ = entry.State.Should().Contain(x => x.Key == "OperationName" && Equals(x.Value, "ImportCustomers"));
    }

    [Fact]
    public void LogDependencyFailure_Should_Write_Error_Log()
    {
        var logger = new TestLogger();
        var exception = new TimeoutException("timeout");

        logger.LogDependencyFailure(exception, "PaymentsApi", "order-1");

        TestLogEntry entry = logger.Entries.Should().ContainSingle().Subject;
        _ = entry.LogLevel.Should().Be(LogLevel.Error);
        _ = entry.EventId.Should().Be(LoggingEventIds.DependencyFailure);
        _ = entry.State.Should().Contain(x => x.Key == "DependencyName" && Equals(x.Value, "PaymentsApi"));
        _ = entry.State.Should().Contain(x => x.Key == "Target" && Equals(x.Value, "order-1"));
    }

    [Fact]
    public void LogResourceNotFound_Should_Write_Information_Log()
    {
        var logger = new TestLogger();

        logger.LogResourceNotFound("Order", 404);

        TestLogEntry entry = logger.Entries.Should().ContainSingle().Subject;
        _ = entry.LogLevel.Should().Be(LogLevel.Information);
        _ = entry.EventId.Should().Be(LoggingEventIds.ResourceNotFound);
        _ = entry.State.Should().Contain(x => x.Key == "ResourceType" && Equals(x.Value, "Order"));
        _ = entry.State.Should().Contain(x => x.Key == "Target" && Equals(x.Value, 404));
    }

    [Fact]
    public void Helpers_Should_Not_Throw_When_Logger_Is_Null()
    {
        ILogger logger = null!;
        var exception = new InvalidOperationException("boom");

        Action[] actions =
        [
            () => logger.LogOperationStarted("Op"),
            () => logger.LogOperationCompleted("Op"),
            () => logger.LogOperationFailed(exception, "Op"),
            () => logger.LogValidationFailed("reason"),
            () => logger.LogUnhandledException(exception),
            () => logger.LogRetryAttempt("Op", 1, 3),
            () => logger.LogDependencyFailure(exception, "Dep"),
            () => logger.LogResourceNotFound("Order"),
        ];

        foreach (Action action in actions)
        {
            _ = action.Should().NotThrow();
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Helpers_Should_Not_Throw_And_Should_Substitute_Placeholder_For_Blank_Names(string? name)
    {
        var logger = new TestLogger();
        var exception = new InvalidOperationException("boom");

        Action action = () =>
        {
            logger.LogOperationStarted(name!);
            logger.LogOperationCompleted(name!);
            logger.LogOperationFailed(exception, name!);
            logger.LogValidationFailed(name!);
            logger.LogRetryAttempt(name!, 1, 3);
            logger.LogDependencyFailure(exception, name!);
            logger.LogResourceNotFound(name!);
        };

        _ = action.Should().NotThrow();
        _ = logger.Entries.Should().HaveCount(7);
        _ = logger.Entries.Should().OnlyContain(entry => !string.IsNullOrWhiteSpace(entry.Message));
    }

    [Fact]
    public void Helpers_Should_Not_Throw_When_Exception_Is_Null()
    {
        var logger = new TestLogger();

        Action action = () =>
        {
            logger.LogOperationFailed(null!, "Op");
            logger.LogUnhandledException(null!);
            logger.LogDependencyFailure(null!, "Dep");
        };

        _ = action.Should().NotThrow();
        _ = logger.Entries.Should().HaveCount(3);
    }

    [Theory]
    [InlineData(0, 3)]
    [InlineData(1, 0)]
    [InlineData(4, 3)]
    [InlineData(-1, -1)]
    public void LogRetryAttempt_Should_Not_Throw_For_Out_Of_Range_Attempts(int attemptNumber, int maxAttempts)
    {
        var logger = new TestLogger();

        Action action = () => logger.LogRetryAttempt("ImportCustomers", attemptNumber, maxAttempts);

        _ = action.Should().NotThrow();
        _ = logger.Entries.Should().ContainSingle();
    }
}
