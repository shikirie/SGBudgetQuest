using System;
using Modules.SavingSystems;
using SimpleJSON;
using UnityEngine;

[Serializable]
public class ActiveSessionData : ISaveable
{
    private SessionDataSummary sessionSummary;
    private float initialAllowance;
    private int initialHappiness;

    public SessionDataSummary SessionSummary => sessionSummary;
    public float InitialAllowance => initialAllowance;
    public int InitialHappiness => initialHappiness;

    public void StartNewSession(GoalData currentGoal)
    {
        initialAllowance = 50;
        initialHappiness = 100;

        sessionSummary = new SessionDataSummary();
        sessionSummary.session_id = Guid.NewGuid().ToString();
        sessionSummary.currentHappiness = initialHappiness;
        sessionSummary.currentSavings = 0;
        sessionSummary.currentWallet = 0;
        sessionSummary.weeklySpentNeeds = 0;
        sessionSummary.weeklySpentWants = 0;
        sessionSummary.currentDay = 1;
        sessionSummary.currentGoal = currentGoal;

        Debug.Log($"[ActiveSessionData] New session started with Session ID: {sessionSummary.session_id}");
        GameplayEvents.OnSessionInitialized?.Invoke();
    }

    public void AddToSavings(float amount)
    {
        sessionSummary.currentSavings += amount;
    }

    public void AddToWallet(float amount)
    {
        sessionSummary.currentWallet += amount;
    }

    public void SpendFromWallet(int amount, bool isNeed)
    {
        if (amount > sessionSummary.currentWallet)
        {
            Debug.LogWarning($"[ActiveSessionData] Not enough funds in wallet to spend {amount}.");

            return;
        }

        sessionSummary.currentWallet -= amount;

        if (isNeed)
        {
            sessionSummary.weeklySpentNeeds += amount;
        }
        else
        {
            sessionSummary.weeklySpentWants += amount;
        }
    }

    #region Serialization (ISaveable)

    public JSONNode AsJSON()
    {
        throw new NotImplementedException();
    }

    public void LoadFromJSON(JSONNode json)
    {
        throw new NotImplementedException();
    }

    #endregion
}

#region Supporting Data Structures

[Serializable]
public class SessionDataSummary
{
    public string session_id;
    public int currentHappiness;
    public float currentSavings;
    public float currentWallet;
    public float weeklySpentNeeds;
    public float weeklySpentWants;
    public int currentDay;
    public GoalData currentGoal;
    public float TotalSpent => weeklySpentNeeds + weeklySpentWants;
}
#endregion
