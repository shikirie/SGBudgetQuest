using System;
using UnityEngine;

public static class GameplayEvents
{
    public static Action OnSessionInitialized;
    public static Action OnButtonChoiceClicked;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        OnSessionInitialized = null;
        OnButtonChoiceClicked = null;
    }
}
