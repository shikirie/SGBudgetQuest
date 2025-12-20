using System;
using UnityEngine;

public static class GameplayEvents
{
    public static Action OnGoalSelected;
    public static Action OnBudgetConfirmed;
    public static Action OnSessionInitialized;
    public static Action OnScenarioStarted;
    public static Action OnWeekEnded;
    public static Action OnPlayerBankrupt;
    public static Action OnGameRestarted;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        OnGoalSelected = null;
        OnBudgetConfirmed = null;
        OnSessionInitialized = null;
        OnScenarioStarted = null;
        OnWeekEnded = null;
        OnPlayerBankrupt = null;
        OnGameRestarted = null;
    }
}
