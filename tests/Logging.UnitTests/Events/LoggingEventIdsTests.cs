using Atya.Diagnostics.Logging.Events;
using Microsoft.Extensions.Logging;

namespace Logging.UnitTests.Events;

public sealed class LoggingEventIdsTests
{
    [Fact]
    public void EventIds_Should_Be_Unique()
    {
        EventId[] values = new[]
        {
            LoggingEventIds.OperationStarted,
            LoggingEventIds.OperationCompleted,
            LoggingEventIds.OperationFailed,
            LoggingEventIds.ValidationFailed,
            LoggingEventIds.RetryAttempt,
            LoggingEventIds.DependencyFailure,
            LoggingEventIds.ResourceNotFound,
            LoggingEventIds.UnexpectedException
        };

        _ = values.Select(x => x.Id).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void EventNames_Should_Be_Stable()
    {
        _ = LoggingEventIds.OperationStarted.Name.Should().Be(nameof(LoggingEventIds.OperationStarted));
        _ = LoggingEventIds.ValidationFailed.Name.Should().Be(nameof(LoggingEventIds.ValidationFailed));
        _ = LoggingEventIds.UnexpectedException.Name.Should().Be(nameof(LoggingEventIds.UnexpectedException));
    }
}
