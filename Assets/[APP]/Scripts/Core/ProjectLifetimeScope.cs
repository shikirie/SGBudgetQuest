using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectLifetimeScope : LifetimeScope
{
    // Databases
    [SerializeField] protected GoalDatabase goalDatabase;
    [SerializeField] protected ScenarioDatabase scenarioDatabase;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(scenarioDatabase);
        builder.RegisterInstance(goalDatabase);

        builder.Register<ActiveSessionData>(Lifetime.Singleton);
        builder.Register<ActiveScenarioData>(Lifetime.Singleton);
        builder.Register<ActiveGoalData>(Lifetime.Singleton);

        // Save system
        builder.RegisterEntryPoint<ProjectSavingSystem>(Lifetime.Singleton).AsSelf();
    }
}
