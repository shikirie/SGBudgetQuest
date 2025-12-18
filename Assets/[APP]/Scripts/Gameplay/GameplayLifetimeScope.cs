using VContainer;
using VContainer.Unity;
using UnityEngine;

public class GameplayLifetimeScope : LifetimeScope
{
    [SerializeField] private GameplayView view;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<GameplayService>(Lifetime.Scoped).AsSelf()
            .WithParameter(view);
    }
}