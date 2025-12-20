using VContainer;
using VContainer.Unity;

public class SplashLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<SplashService>(Lifetime.Scoped).AsSelf();
    }
}