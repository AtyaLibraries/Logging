using Atya.Diagnostics.Logging.Context;
using Atya.Diagnostics.Logging.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Logging.UnitTests.DependencyInjection;

public sealed class LoggingServiceCollectionExtensionsTests
{
    [Fact]
    public void AddAtyaLogging_Should_Register_Package_Services()
    {
        var services = new ServiceCollection();

        _ = services.AddAtyaLogging();

        ServiceProvider provider = services.BuildServiceProvider();

        _ = provider.GetRequiredService<ILogScopeStateFactory>().Should().NotBeNull();
    }

    [Fact]
    public void AddAtyaLogging_Should_Be_Idempotent()
    {
        var services = new ServiceCollection();

        _ = services.AddAtyaLogging();
        _ = services.AddAtyaLogging();

        _ = services.Count(x => x.ServiceType == typeof(ILogScopeStateFactory)).Should().Be(1);
    }

    [Fact]
    public void AddAtyaLogging_Should_Throw_When_Services_Is_Null()
    {
        IServiceCollection services = null!;

        Func<IServiceCollection> action = services.AddAtyaLogging;

        _ = action.Should().Throw<ArgumentNullException>();
    }
}
