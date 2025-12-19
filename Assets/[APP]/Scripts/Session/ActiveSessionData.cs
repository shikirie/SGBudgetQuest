using System;
using SimpleJSON;
using UnityEngine;
using VContainer;

[Serializable]
public class ActiveSessionData
{
    public SessionDataSummary SessionSummary { get; private set; }
    public float InitialAllowance { get; private set; }
    public int InitialHappiness { get; private set; }
    public GoalData CurrentGoal { get; private set; }
    public float TotalWalletSpent { get; private set; }

    [Inject] private readonly GameplaySettings gameplaySettings;

    public void StartNewSession(GoalData currentGoal)
    {
        if (currentGoal == null)
        {
            Debug.LogError("[ActiveSessionData] Cannot start new session: currentGoal is null.");
            return;
        }

        if (gameplaySettings != null)
        {
            Debug.LogError("[ActiveSessionData] Cannot start new session: gameplaySettings is null.");
            return;
        }

        CurrentGoal = currentGoal;
        InitialAllowance = gameplaySettings.InitialAllowance;
        InitialHappiness = gameplaySettings.InitialHappiness;

        SessionSummary = new SessionDataSummary
        {
            sessionId = $"Student_{Guid.NewGuid()}",
            currentHappiness = InitialHappiness,
            currentSavings = 0,
            currentWallet = 0,
            weeklySpentNeeds = 0,
            weeklySpentWants = 0,
            daySurvived = 0,
            goalName = CurrentGoal.GoalName,
            goalCost = CurrentGoal.TargetPrice,
        };

        Debug.Log($"[ActiveSessionData] New session started with Session ID: {SessionSummary.sessionId}");
        GameplayEvents.OnSessionInitialized?.Invoke();
    }

    public void AddToSavings(float amount)
    {
        if (SessionSummary == null)
        {
            Debug.LogWarning("[ActiveSessionData] Cannot add to savings: no active session.");
            return;
        }

        SessionSummary.currentSavings += amount;
    }

    public void AddToWallet(float amount)
    {
        if (SessionSummary == null)
        {
            Debug.LogWarning("[ActiveSessionData] Cannot add to wallet: no active session.");
            return;
        }

        SessionSummary.currentWallet += amount;
    }

    public void SpendFromWallet(ChoiceData choice)
    {
        if (SessionSummary == null)
        {
            Debug.LogWarning("[ActiveSessionData] Cannot spend: no active session.");
            return;
        }

        float cost = choice.ChoiceCost;
        if (cost > SessionSummary.currentWallet)
        {
            Debug.LogWarning($"[ActiveSessionData] Not enough funds in wallet to spend {cost}.");
            return;
        }

        SessionSummary.currentWallet -= cost;

        switch (choice.SpendType)
        {
            case SpendType.Needs:
                SessionSummary.weeklySpentNeeds += cost;
                break;
            case SpendType.Wants:
                SessionSummary.weeklySpentWants += cost;
                break;
        }

        switch (choice.SpendCategory)
        {
            case SpendCategory.Food:
                SessionSummary.weeklyFoodSpent += cost;
                break;
            case SpendCategory.Transport:
                SessionSummary.weeklyTransportSpent += cost;
                break;
            case SpendCategory.Social:
                SessionSummary.weeklySocialSpent += cost;
                break;
            case SpendCategory.Shopping:
                SessionSummary.weeklyShoppingSpent += cost;
                break;
        }

        TotalWalletSpent = SessionSummary.totalWalletSpent += cost;
    }

    public void UpdateHappiness(ChoiceData choice)
    {
        if (SessionSummary == null)
        {
            Debug.LogWarning("[ActiveSessionData] Cannot update happiness: no active session.");
            return;
        }

        SessionSummary.currentHappiness = Math.Clamp(SessionSummary.currentHappiness + choice.ChoiceHappiness, 0, 100);
    }
}

#region Supporting Data Structures

[Serializable]
public class SessionDataSummary
{
    public string sessionId;

    public int currentHappiness;
    public float currentSavings;
    public float currentWallet;

    public float weeklySpentNeeds;
    public float weeklySpentWants;
    public float weeklyFoodSpent;
    public float weeklyTransportSpent;
    public float weeklySocialSpent;
    public float weeklyShoppingSpent;

    public float totalWalletSpent;

    public int daySurvived;

    public string goalName;
    public float goalCost;

    public bool isWin;
}

#endregion
