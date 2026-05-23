using Atya.Diagnostics.Logging.Context;

namespace Logging.UnitTests.Context;

public sealed class LogScopeStateTests
{
    [Fact]
    public void Empty_Should_Return_Reusable_Empty_State()
    {
        _ = LogScopeState.Empty.Count.Should().Be(0);
        _ = LogScopeState.Empty.ToString().Should().BeEmpty();
    }

    [Fact]
    public void Constructor_FromKeyValuePairs_Should_Filter_Blank_Property_Names()
    {
        KeyValuePair<string, object?>[] properties =
        [
            new KeyValuePair<string, object?>("CorrelationId", "corr-1"),
            new KeyValuePair<string, object?>("", "ignored"),
            new KeyValuePair<string, object?>(" ", "ignored"),
            new KeyValuePair<string, object?>("OperationName", "ImportCustomers")
        ];

        LogScopeState state = new(properties);

        _ = state.Count.Should().Be(2);
        _ = state[0].Should().Be(new KeyValuePair<string, object?>("CorrelationId", "corr-1"));
        _ = state[1].Should().Be(new KeyValuePair<string, object?>("OperationName", "ImportCustomers"));
    }

    [Fact]
    public void Constructor_FromContextProperties_Should_Create_KeyValuePairs()
    {
        LogContextProperty[] properties =
        [
            new(KnownLogPropertyNames.TenantId, "tenant-1"),
            new(KnownLogPropertyNames.UserId, "user-1")
        ];

        LogScopeState state = new(properties);

        _ = state.Should().ContainInOrder(
            new KeyValuePair<string, object?>(KnownLogPropertyNames.TenantId, "tenant-1"),
            new KeyValuePair<string, object?>(KnownLogPropertyNames.UserId, "user-1"));
    }

    [Fact]
    public void Constructor_FromKeyValuePairs_Should_Throw_When_Properties_Is_Null()
    {
        IEnumerable<KeyValuePair<string, object?>> properties = null!;

        Action action = () => _ = new LogScopeState(properties);

        _ = action.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName == "properties");
    }

    [Fact]
    public void Constructor_FromContextProperties_Should_Throw_When_Properties_Is_Null()
    {
        IEnumerable<LogContextProperty> properties = null!;

        Action action = () => _ = new LogScopeState(properties);

        _ = action.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName == "properties");
    }

    [Fact]
    public void GetEnumerator_Should_Enumerate_All_Properties()
    {
        LogScopeState state = new(
        [
            new KeyValuePair<string, object?>("A", 1),
            new KeyValuePair<string, object?>("B", 2)
        ]);

        List<KeyValuePair<string, object?>> properties = [];
        using IEnumerator<KeyValuePair<string, object?>> enumerator = state.GetEnumerator();
        while (enumerator.MoveNext())
        {
            properties.Add(enumerator.Current);
        }

        _ = properties.Should().Equal(
            new KeyValuePair<string, object?>("A", 1),
            new KeyValuePair<string, object?>("B", 2));
    }

    [Fact]
    public void NonGenericEnumerator_Should_Enumerate_All_Properties()
    {
        var state = new LogScopeState(
        [
            new KeyValuePair<string, object?>("A", 1)
        ]);
        System.Collections.IEnumerator enumerator = ((System.Collections.IEnumerable)state).GetEnumerator();

        try
        {
            _ = enumerator.MoveNext().Should().BeTrue();
            _ = enumerator.Current.Should().Be(new KeyValuePair<string, object?>("A", 1));
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }
    }

    [Fact]
    public void ToString_Should_Render_Name_Value_Pairs()
    {
        LogScopeState state = new(
        [
            new KeyValuePair<string, object?>("CorrelationId", "corr-1"),
            new KeyValuePair<string, object?>("EntityId", 42),
            new KeyValuePair<string, object?>("Optional", null)
        ]);

        _ = state.ToString().Should().Be("CorrelationId=corr-1, EntityId=42, Optional=");
    }
}
