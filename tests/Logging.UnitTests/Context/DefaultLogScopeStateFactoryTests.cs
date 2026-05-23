using Atya.Diagnostics.Logging.Context;

namespace Logging.UnitTests.Context;

public sealed class DefaultLogScopeStateFactoryTests
{
    [Fact]
    public void Create_Should_Return_Normalized_Scope_State()
    {
        var factory = new DefaultLogScopeStateFactory();

        LogScopeState state = factory.Create(
        [
            new KeyValuePair<string, object?>("CorrelationId", "corr-1"),
            new KeyValuePair<string, object?>("", "ignored")
        ]);

        _ = state.Should().ContainSingle()
            .Which.Should().Be(new KeyValuePair<string, object?>("CorrelationId", "corr-1"));
    }

    [Fact]
    public void Create_Should_Throw_When_Properties_Is_Null()
    {
        var factory = new DefaultLogScopeStateFactory();
        IEnumerable<KeyValuePair<string, object?>> properties = null!;

        Action action = () => _ = factory.Create(properties);

        _ = action.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName == "properties");
    }
}
