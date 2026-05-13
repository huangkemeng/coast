using Autofac;

namespace RequirementTrackingSystem.UnitTests;

public static class TestEnvironmentCache
{
    public static ILifetimeScope? LifetimeScope { get; set; }
}