using VContainer;
using VContainer.Unity;
using UnityEngine;

public class MainMenuLifetimeScope : LifetimeScope
{
    [SerializeField] private MainMenuView view;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<MainMenuService>(Lifetime.Scoped).AsSelf()
            .WithParameter(view);
    }
}