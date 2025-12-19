using System;
using VContainer;
using VContainer.Unity;

public class GameplayService : IInitializable, IStartable, IPostStartable, ITickable, IDisposable
{
    [Inject] public readonly ActiveSessionData ActiveSessionData;
    [Inject] public readonly ActiveScenarioData ActiveScenarioData;
    [Inject] public readonly ActiveGoalData ActiveGoalData;

    public GameplayView View { get; private set; }

    private readonly SubService[] subServices = new SubService[]
    {
        new GameplayGoalService(),
        new GameplayBudgetingService(),
        new GameplayScenarioService(),
        new GameplayStatusService(),
        new GameplayReportService(),
    };

    public GameplayService(GameplayView view)
    {
        View = view;
    }

    void IInitializable.Initialize()
    {
        for (int i = 0; i < subServices.Length; i++)
        {
            subServices[i].Initialize(this);
        }
    }

    void IStartable.Start()
    {
    }

    void IPostStartable.PostStart()
    {
        for (int i = 0; i < subServices.Length; i++)
        {
            subServices[i].Start();
        }
    }

    void ITickable.Tick()
    {
        for (int i = 0; i < subServices.Length; i++)
        {
            subServices[i].Tick();
        }
    }

    void IDisposable.Dispose()
    {
        for (int i = 0; i < subServices.Length; i++)
        {
            subServices[i].Dispose();
        }
    }

    public T GetService<T>() where T : SubService
    {
        for (int i = 0; i < subServices.Length; i++)
        {
            if (subServices[i] is T service)
            {
                return service;
            }
        }
        return null;
    }
}