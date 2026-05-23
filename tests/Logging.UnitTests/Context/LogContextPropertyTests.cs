using Atya.Diagnostics.Logging.Context;

namespace Logging.UnitTests.Context;

public sealed class LogContextPropertyTests
{
    [Fact]
    public void Constructor_Should_Set_Properties()
    {
        LogContextProperty property = new("CorrelationId", "corr-1");

        _ = property.Name.Should().Be("CorrelationId");
        _ = property.Value.Should().Be("corr-1");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_Should_Throw_When_Name_Is_Invalid(string? name)
    {
        Action action = () => _ = new LogContextProperty(name!, "value");

        _ = action.Should().Throw<ArgumentException>().Where(exception => exception.ParamName == "name");
    }

    [Fact]
    public void ToKeyValuePair_Should_Return_Name_And_Value()
    {
        LogContextProperty property = new("TenantId", "tenant-1");

        KeyValuePair<string, object?> keyValuePair = property.ToKeyValuePair();

        _ = keyValuePair.Key.Should().Be("TenantId");
        _ = keyValuePair.Value.Should().Be("tenant-1");
    }

    [Fact]
    public void Equality_Should_Use_Name_And_Value()
    {
        LogContextProperty left = new("UserId", "user-1");
        LogContextProperty same = new("UserId", "user-1");
        LogContextProperty different = new("UserId", "user-2");

        _ = left.Equals(same).Should().BeTrue();
        _ = left.Equals((object)same).Should().BeTrue();
        _ = (left == same).Should().BeTrue();
        _ = (left != different).Should().BeTrue();
        _ = left.Equals(different).Should().BeFalse();
        _ = left.GetHashCode().Should().Be(same.GetHashCode());
    }

    [Fact]
    public void Default_Value_Should_Have_Null_Name_And_Value()
    {
        LogContextProperty property = default;

        _ = property.Name.Should().BeNull();
        _ = property.Value.Should().BeNull();
    }
}
