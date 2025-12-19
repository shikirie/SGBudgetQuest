using System;
using UnityEngine;

public static class GameplayEvents
{
    public static Action OnGoalSelected;
    public static Action OnSessionInitialized;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        OnGoalSelected = null;
        OnSessionInitialized = null;
    }
}
