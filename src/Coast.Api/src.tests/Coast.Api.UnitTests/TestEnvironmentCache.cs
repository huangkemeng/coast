using Autofac;

namespace Coast.Api.UnitTests;

public static class TestEnvironmentCache
{
    public static ILifetimeScope? LifetimeScope { get; set; }
}