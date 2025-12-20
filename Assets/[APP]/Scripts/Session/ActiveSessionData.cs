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

    /// <summary>
    /// Initialize initial values from settings. Call this before showing goal selection.
    /// </summary>
    public void Initialize()
    {
        if (gameplaySettings == null)
        {
            Debug.LogError("[ActiveSessionData] Cannot initialize: gameplaySettings is null.");
            return;
        }

        InitialAllowance = gameplaySettings.InitialAllowance;
        InitialHappiness = gameplaySettings.InitialHappiness;
        
        Debug.Log($"[ActiveSessionData] Initialized with allowance: S${InitialAllowance}, happiness: {InitialHappiness}");
    }

    public void StartNewSession(GoalData currentGoal)
    {
        if (currentGoal == null)
        {
            Debug.LogError("[ActiveSessionData] Cannot start new session: currentGoal is null.");
            return;
        }

        if (gameplaySettings == null)
        {
            Debug.LogError("[ActiveSessionData] Cannot start new session: gameplaySettings is null.");
            return;
        }

        CurrentGoal = currentGoal;

        SessionSummary = new SessionDataSummary
        {
            sessionId = $"Student_{Guid.NewGuid()}",
            initialAllowance = InitialAllowance,
            initialHappiness = InitialHappiness,
            currentHappiness = InitialHappiness,
            currentSavings = 0,
            currentWallet = 0,
            weeklySpentNeeds = 0,
            weeklySpentWants = 0,
            daySurvived = 1,
            goalName = CurrentGoal.GoalName,
            goalCost = CurrentGoal.TargetPrice,
        };

        Debug.Log($"[ActiveSessionData] New session started with Session ID: {SessionSummary.sessionId}");
        GameplayEvents.OnSessionInitialized?.Invoke();
    }

    public int GetCurrentDay()
    {
        return SessionSummary != null ? SessionSummary.daySurvived : 1;
    }

    public void IncrementDay()
    {
        if (SessionSummary == null)
        {
            Debug.LogWarning("[ActiveSessionData] Cannot increment day: no active session.");
            return;
        }

        SessionSummary.daySurvived++;
        Debug.Log($"[ActiveSessionData] Day incremented to {SessionSummary.daySurvived}");
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

        if (SessionSummary.currentWallet < 0.01f)
        {
            SessionSummary.currentWallet = 0f;
        }

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

    public bool CheckBankruptcy()
    {
        if (SessionSummary == null)
        {
            return false;
        }

        return SessionSummary.currentWallet < 0.01f;
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

    public void SetSessionStatus()
    {
        if (SessionSummary == null)
        {
            Debug.LogWarning("[ActiveSessionData] Cannot set session status: no active session.");
            return;
        }

        SessionStatus status = GetSessionStatus();
        SessionSummary.sessionStatus = status.ToString();
    }

    public SessionStatus GetSessionStatus()
    {
        if (SessionSummary == null)
        {
            Debug.LogWarning("[ActiveSessionData] Cannot get session status: no active session.");
            return SessionStatus.Poor;
        }

        // Check bankruptcy first
        if (SessionSummary.currentWallet <= 0)
        {
            return SessionStatus.Bankrupt;
        }

        float savings = SessionSummary.currentSavings;
        float target = SessionSummary.goalCost;
        int happiness = SessionSummary.currentHappiness;

        // 1. PERFECT (God Tier): Win & Happy
        if (savings >= target && happiness >= 50)
        {
            return SessionStatus.Perfect;
        }
        // 2. GOOD (Burnout): Win but Stressed (Note the '<' sign in happiness)
        else if (savings >= target && happiness < 50)
        {
            return SessionStatus.Good;
        }
        // 3. AVERAGE (Impulse Buyer): Lose but Happy
        else if (savings < target && happiness >= 50)
        {
            return SessionStatus.Average;
        }
        // 4. POOR (Disaster): Lose & Stress
        else
        {
            return SessionStatus.Poor;
        }
    }

    public void SetLeakiestCategory()
    {
        if (SessionSummary == null)
        {
            Debug.LogWarning("[ActiveSessionData] Cannot set leakiest category: no active session.");
            return;
        }

        SpendCategory leakiest = GetLeakiestCategory();
        SessionSummary.expenseLeakCategory = leakiest.ToString();
    }

    public SpendCategory GetLeakiestCategory()
    {
        if (SessionSummary == null)
        {
            Debug.LogWarning("[ActiveSessionData] Cannot get leakiest category: no active session.");
            return SpendCategory.None;
        }

        float maxSpent = Math.Max(Math.Max(SessionSummary.weeklyFoodSpent, SessionSummary.weeklyTransportSpent),
                                  Math.Max(SessionSummary.weeklySocialSpent, SessionSummary.weeklyShoppingSpent));

        if (maxSpent == 0)
        {
            return SpendCategory.None;
        }
        else if (maxSpent == SessionSummary.weeklyFoodSpent)
        {
            return SpendCategory.Food;
        }
        else if (maxSpent == SessionSummary.weeklyTransportSpent)
        {
            return SpendCategory.Transport;
        }
        else if (maxSpent == SessionSummary.weeklySocialSpent)
        {
            return SpendCategory.Social;
        }
        else // maxSpent == SessionSummary.weeklyShoppingSpent
        {
            return SpendCategory.Shopping;
        }
    }

    public float GetLeakiestCategoryValue()
    {
        if (SessionSummary == null)
        {
            Debug.LogWarning("[ActiveSessionData] Cannot get leakiest category value: no active session.");
            return 0;
        }

        SpendCategory leakiest = GetLeakiestCategory();
        switch (leakiest)
        {
            case SpendCategory.Food:
                return SessionSummary.weeklyFoodSpent;
            case SpendCategory.Transport:
                return SessionSummary.weeklyTransportSpent;
            case SpendCategory.Social:
                return SessionSummary.weeklySocialSpent;
            case SpendCategory.Shopping:
                return SessionSummary.weeklyShoppingSpent;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Convert session data to JSON format for API submission.
    /// </summary>
    public string ToJson()
    {
        if (SessionSummary == null)
        {
            Debug.LogWarning("[ActiveSessionData] Cannot convert to JSON: no active session.");
            return "{}";
        }

        // Finalize session data
        SetSessionStatus();
        SetLeakiestCategory();

        JSONNode json = new JSONObject();
        json["sessionId"] = SessionSummary.sessionId;
        json["initialAllowance"] = (float)Math.Round(SessionSummary.initialAllowance, 2);
        json["initialHappiness"] = SessionSummary.initialHappiness;
        json["currentHappiness"] = SessionSummary.currentHappiness;
        json["currentSavings"] = (float)Math.Round(SessionSummary.currentSavings, 2);
        json["currentWallet"] = (float)Math.Round(SessionSummary.currentWallet, 2);
        json["weeklySpentNeeds"] = (float)Math.Round(SessionSummary.weeklySpentNeeds, 2);
        json["weeklySpentWants"] = (float)Math.Round(SessionSummary.weeklySpentWants, 2);
        json["weeklyFoodSpent"] = (float)Math.Round(SessionSummary.weeklyFoodSpent, 2);
        json["weeklyTransportSpent"] = (float)Math.Round(SessionSummary.weeklyTransportSpent, 2);
        json["weeklySocialSpent"] = (float)Math.Round(SessionSummary.weeklySocialSpent, 2);
        json["weeklyShoppingSpent"] = (float)Math.Round(SessionSummary.weeklyShoppingSpent, 2);
        json["totalWalletSpent"] = (float)Math.Round(SessionSummary.totalWalletSpent, 2);
        json["daySurvived"] = Math.Min(SessionSummary.daySurvived, 7);
        json["goalName"] = SessionSummary.goalName;
        json["goalCost"] = (float)Math.Round(SessionSummary.goalCost, 2);
        json["sessionStatus"] = SessionSummary.sessionStatus;
        json["expenseLeakCategory"] = SessionSummary.expenseLeakCategory;

        return json.ToString();
    }
}
#region Supporting Data Structures

[Serializable]
public class SessionDataSummary
{
    public string sessionId;

    public float initialAllowance;
    public int initialHappiness;
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

    public string sessionStatus;
    public string expenseLeakCategory;
}

#endregion

public enum SessionStatus
{
    Perfect,
    Good,
    Average,
    Poor,
    Bankrupt
}